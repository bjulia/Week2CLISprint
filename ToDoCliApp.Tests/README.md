# Edge Case Tests for ToDo CLI Application

This file contains comprehensive edge case tests for the ToDo CLI application, specifically focusing on adding and editing ToDo items.

## Test Overview

### Test 1: Adding ToDo Items with Extreme Input Values
**Purpose:** Tests boundary conditions and extreme input scenarios
**Edge Cases Covered:**
- Very long strings (10,000+ characters) to test buffer overflow protection
- Special characters and Unicode text (emojis, international characters)
- Extreme dates (DateTime.MaxValue, DateTime.MinValue)
- Empty strings vs null values
- HTML/XML special characters that could cause parsing issues

### Test 2: Editing ToDo Items with Concurrent ID Conflicts
**Purpose:** Tests ID management and concurrent access scenarios
**Edge Cases Covered:**
- Duplicate ID scenarios (multiple items with same ID)
- Editing non-existent items (ID not found)
- Negative and zero ID values
- Concurrent editing of the same item
- Race conditions in ID assignment

### Test 3: JSON Serialization/Deserialization with Corrupted Data
**Purpose:** Tests data persistence and error handling
**Edge Cases Covered:**
- Corrupted JSON files
- Empty files
- Missing required fields in JSON
- Very large datasets (performance testing)
- Special characters in JSON serialization

## How to Run Tests

### Prerequisites
1. Install .NET 8.0 SDK
2. Install MSTest framework packages

### Running the Tests

```powershell
# Navigate to the test project directory
cd "d:\Users\aajuliab\Projects\ChatGPTBobJ\Week2CLISprint\ToDoCliApp.Tests"

# Restore packages
dotnet restore

# Build the test project
dotnet build

# Run all tests
dotnet test

# Run tests with verbose output
dotnet test --verbosity normal

# Run specific test method
dotnet test --filter "TestMethodName=AddToDoItem_ExtremeInputValues_HandlesEdgeCases"
```

### Test Results Interpretation

**Green (Pass):** The edge case is handled correctly
**Red (Fail):** The edge case exposes a bug or limitation that needs fixing

## Key Edge Cases Identified

1. **Buffer Overflow Protection:** Tests with 10,000+ character strings
2. **Unicode Support:** International characters and emojis
3. **Date Boundary Conditions:** Maximum and minimum DateTime values
4. **ID Collision Handling:** Multiple items with same ID
5. **Data Corruption Recovery:** Malformed JSON files
6. **Performance Under Load:** Large datasets (10,000+ items)
7. **Null vs Empty String Handling:** Different empty value types
8. **Concurrent Access:** Multiple edits to same item

## Expected Behaviors

- Application should gracefully handle all extreme inputs without crashing
- Data integrity should be maintained even with corrupted files
- ID conflicts should be resolved consistently
- Performance should remain reasonable with large datasets
- Error messages should be informative and user-friendly

## Integration with Main Application

These tests complement the main application by:
- Validating the robustness of the AddItem() method
- Testing the EditItem() method under stress conditions
- Ensuring JSON persistence works with edge cases
- Verifying ID generation logic handles gaps correctly

Run these tests regularly during development to catch edge case regressions early.
