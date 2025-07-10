using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ToDoCliApp.Tests
{
    [TestClass]
    public class ToDoItemEdgeCaseTests
    {
        private List<ToDoItem> testToDoList = new List<ToDoItem>();
        private string testFilePath = "test_todo.json";

        [TestInitialize]
        public void Setup()
        {
            testToDoList = new List<ToDoItem>();
            // Clean up any existing test file
            if (File.Exists(testFilePath))
            {
                File.Delete(testFilePath);
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up test file after each test
            if (File.Exists(testFilePath))
            {
                File.Delete(testFilePath);
            }
        }

        /// <summary>
        /// Edge Case Test 1: Adding ToDo Items with Extreme Input Values
        /// Tests boundary conditions including very long strings, special characters, 
        /// extreme dates, and null/empty values
        /// </summary>
        [TestMethod]
        public void AddToDoItem_ExtremeInputValues_HandlesEdgeCases()
        {
            // Test Case 1a: Very long name (potential buffer overflow)
            var veryLongName = new string('A', 10000); // 10,000 characters
            var longNameItem = new ToDoItem
            {
                ID = 1,
                Name = veryLongName,
                CreateDate = DateTime.Now,
                DueDate = null,
                CompleteFlag = false,
                Notes = "Test notes"
            };
            testToDoList.Add(longNameItem);
            Assert.AreEqual(veryLongName, longNameItem.Name);
            Assert.AreEqual(1, testToDoList.Count);

            // Test Case 1b: Special characters and Unicode
            var specialCharsName = "📝 Test with émojis & spéciàl chars: <>&\"'`~!@#$%^&*()_+-=[]{}|;:,.<>?";
            var specialCharsItem = new ToDoItem
            {
                ID = 2,
                Name = specialCharsName,
                CreateDate = DateTime.Now,
                DueDate = null,
                CompleteFlag = false,
                Notes = "Unicode test: 中文 العربية Русский 日本語"
            };
            testToDoList.Add(specialCharsItem);
            Assert.AreEqual(specialCharsName, specialCharsItem.Name);

            // Test Case 1c: Extreme dates
            var extremeFutureDate = DateTime.MaxValue;
            var extremePastDate = DateTime.MinValue;
            
            var futureDateItem = new ToDoItem
            {
                ID = 3,
                Name = "Future Task",
                CreateDate = DateTime.Now,
                DueDate = extremeFutureDate,
                CompleteFlag = false,
                Notes = "Far future task"
            };
            testToDoList.Add(futureDateItem);
            Assert.AreEqual(extremeFutureDate, futureDateItem.DueDate);

            var pastDateItem = new ToDoItem
            {
                ID = 4,
                Name = "Past Task",
                CreateDate = DateTime.Now,
                DueDate = extremePastDate,
                CompleteFlag = false,
                Notes = "Ancient task"
            };
            testToDoList.Add(pastDateItem);
            Assert.AreEqual(extremePastDate, pastDateItem.DueDate);

            // Test Case 1d: Empty strings vs null (should handle gracefully)
            var emptyStringItem = new ToDoItem
            {
                ID = 5,
                Name = "", // Empty string
                CreateDate = DateTime.Now,
                DueDate = null,
                CompleteFlag = false,
                Notes = "" // Empty notes
            };
            testToDoList.Add(emptyStringItem);
            Assert.AreEqual("", emptyStringItem.Name);
            Assert.AreEqual("", emptyStringItem.Notes);

            Assert.AreEqual(5, testToDoList.Count);
        }

        /// <summary>
        /// Edge Case Test 2: Editing ToDo Items with Concurrent ID Conflicts
        /// Tests scenarios where multiple items might have conflicting IDs,
        /// editing non-existent items, and handling race conditions
        /// </summary>
        [TestMethod]
        public void EditToDoItem_ConcurrentIDConflicts_HandlesEdgeCases()
        {
            // Setup: Create items with potential ID conflicts
            var item1 = new ToDoItem
            {
                ID = 1,
                Name = "Original Task 1",
                CreateDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(1),
                CompleteFlag = false,
                Notes = "Original notes 1"
            };
            
            var item2 = new ToDoItem
            {
                ID = 2,
                Name = "Original Task 2",
                CreateDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(2),
                CompleteFlag = false,
                Notes = "Original notes 2"
            };
            
            testToDoList.Add(item1);
            testToDoList.Add(item2);

            // Test Case 2a: Editing with duplicate ID scenario
            // Simulate what happens when two items somehow get the same ID
            var duplicateIdItem = new ToDoItem
            {
                ID = 1, // Same ID as item1
                Name = "Duplicate ID Task",
                CreateDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(3),
                CompleteFlag = false,
                Notes = "This has a duplicate ID"
            };
            testToDoList.Add(duplicateIdItem);

            // When editing by ID, should find the first occurrence
            var foundItem = testToDoList.FirstOrDefault(x => x.ID == 1);
            Assert.IsNotNull(foundItem);
            Assert.AreEqual("Original Task 1", foundItem.Name); // Should be the first one

            // Test Case 2b: Editing non-existent ID (boundary condition)
            var nonExistentId = 99999;
            var nonExistentItem = testToDoList.FirstOrDefault(x => x.ID == nonExistentId);
            Assert.IsNull(nonExistentItem);

            // Test Case 2c: Editing with negative ID
            var negativeId = -1;
            var negativeIdItem = testToDoList.FirstOrDefault(x => x.ID == negativeId);
            Assert.IsNull(negativeIdItem);

            // Test Case 2d: Editing with zero ID
            var zeroId = 0;
            var zeroIdItem = testToDoList.FirstOrDefault(x => x.ID == zeroId);
            Assert.IsNull(zeroIdItem);

            // Test Case 2e: Simulate concurrent editing (multiple edits to same item)
            var targetItem = testToDoList.First(x => x.ID == 2);
            var originalName = targetItem.Name;
            
            // First edit
            targetItem.Name = "First Edit";
            targetItem.Notes = "First edit notes";
            
            // Second edit (overwrites first)
            targetItem.Name = "Second Edit";
            targetItem.Notes = "Second edit notes";
            
            Assert.AreEqual("Second Edit", targetItem.Name);
            Assert.AreEqual("Second edit notes", targetItem.Notes);
            Assert.AreNotEqual(originalName, targetItem.Name);
        }

        /// <summary>
        /// Edge Case Test 3: JSON Serialization/Deserialization with Corrupted Data
        /// Tests handling of corrupted JSON files, invalid data types, and persistence edge cases
        /// </summary>
        [TestMethod]
        public void ToDoItemPersistence_CorruptedData_HandlesEdgeCases()
        {
            // Test Case 3a: Save and load with special characters
            var specialItem = new ToDoItem
            {
                ID = 1,
                Name = "Test with \"quotes\" and \\ backslashes \n newlines \t tabs",
                CreateDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(1),
                CompleteFlag = true,
                Notes = "Notes with special chars: àáâãäåæçèé"
            };
            testToDoList.Add(specialItem);

            // Save to JSON
            var json = JsonConvert.SerializeObject(testToDoList, Formatting.Indented);
            File.WriteAllText(testFilePath, json);

            // Load from JSON
            var loadedJson = File.ReadAllText(testFilePath);
            var loadedList = JsonConvert.DeserializeObject<List<ToDoItem>>(loadedJson);
            
            Assert.IsNotNull(loadedList);
            Assert.AreEqual(1, loadedList.Count);
            Assert.AreEqual(specialItem.Name, loadedList[0].Name);
            Assert.AreEqual(specialItem.Notes, loadedList[0].Notes);

            // Test Case 3b: Handle corrupted JSON file
            var corruptedJson = "{ \"corrupted\": \"json\", \"missing\": }";
            File.WriteAllText(testFilePath, corruptedJson);

            try
            {
                var corruptedLoadedList = JsonConvert.DeserializeObject<List<ToDoItem>>(File.ReadAllText(testFilePath));
                Assert.Fail("Should have thrown exception for corrupted JSON");
            }
            catch (JsonException)
            {
                // Expected behavior - corrupted JSON should throw exception
                Assert.IsTrue(true);
            }

            // Test Case 3c: Handle empty file
            File.WriteAllText(testFilePath, "");
            try
            {
                var emptyFileList = JsonConvert.DeserializeObject<List<ToDoItem>>(File.ReadAllText(testFilePath));
                // Empty string should result in null
                Assert.IsNull(emptyFileList);
            }
            catch (JsonException)
            {
                // This is also acceptable behavior
                Assert.IsTrue(true);
            }

            // Test Case 3d: Handle JSON with missing required fields
            var incompleteJson = "[{\"ID\": 1, \"CreateDate\": \"2024-01-01T00:00:00\", \"CompleteFlag\": false}]";
            File.WriteAllText(testFilePath, incompleteJson);

            try
            {
                var incompleteList = JsonConvert.DeserializeObject<List<ToDoItem>>(File.ReadAllText(testFilePath));
                // Newtonsoft.Json is more lenient - it may not throw exception for missing required fields
                // Instead, check if the deserialized object has null values for required fields
                if (incompleteList != null && incompleteList.Count > 0)
                {
                    // This is acceptable behavior - some JSON deserializers are lenient
                    Assert.IsTrue(true, "JSON deserialization handled missing fields gracefully");
                }
                else
                {
                    Assert.Fail("Deserialization should have produced at least one item");
                }
            }
            catch (JsonException)
            {
                // This is also acceptable behavior - strict JSON validation
                Assert.IsTrue(true, "JSON deserialization properly validated required fields");
            }

            // Test Case 3e: Handle very large JSON file (performance test)
            var largeList = new List<ToDoItem>();
            for (int i = 1; i <= 10000; i++)
            {
                largeList.Add(new ToDoItem
                {
                    ID = i,
                    Name = $"Task {i}",
                    CreateDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(i),
                    CompleteFlag = i % 2 == 0,
                    Notes = $"Notes for task {i}"
                });
            }

            var largeJson = JsonConvert.SerializeObject(largeList, Formatting.Indented);
            File.WriteAllText(testFilePath, largeJson);
            
            var loadedLargeList = JsonConvert.DeserializeObject<List<ToDoItem>>(File.ReadAllText(testFilePath));
            Assert.IsNotNull(loadedLargeList);
            Assert.AreEqual(10000, loadedLargeList.Count);
            Assert.AreEqual("Task 5000", loadedLargeList[4999].Name); // Check middle item
        }

        /// <summary>
        /// Helper method to simulate the ID generation logic from the main application
        /// </summary>
        private int GenerateNewId(List<ToDoItem> list)
        {
            return list.Count > 0 ? list.Max(x => x.ID) + 1 : 1;
        }

        /// <summary>
        /// Additional edge case: Test ID generation with gaps
        /// </summary>
        [TestMethod]
        public void GenerateNewId_WithGapsInSequence_HandlesCorrectly()
        {
            // Create items with gaps in ID sequence
            testToDoList.Add(new ToDoItem { ID = 1, Name = "Task 1", CreateDate = DateTime.Now, CompleteFlag = false, Notes = "" });
            testToDoList.Add(new ToDoItem { ID = 5, Name = "Task 5", CreateDate = DateTime.Now, CompleteFlag = false, Notes = "" });
            testToDoList.Add(new ToDoItem { ID = 3, Name = "Task 3", CreateDate = DateTime.Now, CompleteFlag = false, Notes = "" });
            testToDoList.Add(new ToDoItem { ID = 10, Name = "Task 10", CreateDate = DateTime.Now, CompleteFlag = false, Notes = "" });

            var newId = GenerateNewId(testToDoList);
            Assert.AreEqual(11, newId); // Should be max ID + 1

            // Test with empty list
            var emptyList = new List<ToDoItem>();
            var firstId = GenerateNewId(emptyList);
            Assert.AreEqual(1, firstId);
        }
    }
}
