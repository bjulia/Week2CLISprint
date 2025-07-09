
using System.Text.Json;
using System.Text.Json.Serialization;

class Program
{
    const string FileName = "todo.json";
    static List<ToDoItem> ToDoList = new();

    static void Main()
    {
        LoadToDoList();
        while (true)
        {
            Console.WriteLine("\nTo-Do List CLI");
            Console.WriteLine("1. List all to-do items");
            Console.WriteLine("2. Add a new to-do item");
            Console.WriteLine("3. Edit a to-do item");
            Console.WriteLine("4. Delete a to-do item");
            Console.WriteLine("5. Mark as complete/incomplete");
            Console.WriteLine("6. Exit");
            Console.Write("Select an option: ");
            var input = Console.ReadLine();
            switch (input)
            {
                case "1": ListItems(); break;
                case "2": AddItem(); break;
                case "3": EditItem(); break;
                case "4": DeleteItem(); break;
                case "5": ToggleComplete(); break;
                case "6": SaveToDoList(); return;
                default: Console.WriteLine("Invalid option. Try again."); break;
            }
        }
    }

    static void LoadToDoList()
    {
        if (File.Exists(FileName))
        {
            try
            {
                var json = File.ReadAllText(FileName);
                ToDoList = JsonSerializer.Deserialize<List<ToDoItem>>(json) ?? new List<ToDoItem>();
            }
            catch
            {
                Console.WriteLine("Error loading todo.json. Starting with an empty list.");
                ToDoList = new List<ToDoItem>();
            }
        }
    }

    static void SaveToDoList()
    {
        try
        {
            var json = JsonSerializer.Serialize(ToDoList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FileName, json);
        }
        catch
        {
            Console.WriteLine("Error saving to todo.json.");
        }
    }

    static void ListItems()
    {
        if (ToDoList.Count == 0)
        {
            Console.WriteLine("No to-do items found.");
            return;
        }

        // Table header
        string header = string.Format("| {0,3} | {1,-20} | {2,-16} | {3,-10} | {4,-5} | {5,-30} |", "#", "Name", "Created", "Due", "Done", "Notes");
        string separator = new string('-', header.Length);
        Console.WriteLine("\n" + separator);
        Console.WriteLine(header);
        Console.WriteLine(separator);

        foreach (var item in ToDoList)
        {
            string due = item.DueDate.HasValue ? item.DueDate.Value.ToString("yyyy-MM-dd") : "-";
            string done = item.CompleteFlag ? "[x]" : "[ ]"; // [x] for complete, [ ] for incomplete
            string notes = string.IsNullOrWhiteSpace(item.Notes) ? "" : item.Notes.Length > 28 ? item.Notes.Substring(0, 27) + "…" : item.Notes;
            // Print ID in blue
            Console.Write("| ");
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"{item.ID,3}");
            Console.ForegroundColor = prevColor;
            Console.WriteLine(string.Format(" | {0,-20} | {1:yyyy-MM-dd HH:mm} | {2,-10} | {3,-5} | {4,-30} |", item.Name, item.CreateDate, due, done, notes));
        }
        Console.WriteLine(separator);
    }

    static void AddItem()
    {
        Console.Write("Enter name: ");
        var name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Name cannot be empty.");
            return;
        }
        Console.Write("Enter due date (yyyy-MM-dd) or leave blank: ");
        DateTime? dueDate = null;
        var dueInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(dueInput))
        {
            if (DateTime.TryParse(dueInput, out var dt))
                dueDate = dt;
            else
            {
                Console.WriteLine("Invalid date format.");
                return;
            }
        }
        Console.Write("Enter notes: ");
        var notes = Console.ReadLine();
        int newId = ToDoList.Count > 0 ? ToDoList.Max(x => x.ID) + 1 : 1;
        var item = new ToDoItem
        {
            ID = newId,
            Name = name,
            CreateDate = DateTime.Now,
            DueDate = dueDate,
            CompleteFlag = false,
            Notes = notes ?? ""
        };
        ToDoList.Add(item);
        SaveToDoList();
        Console.WriteLine("To-do item added.");
    }

    static void EditItem()
    {
        Console.Write("Enter ID of item to edit: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }
        var item = ToDoList.FirstOrDefault(x => x.ID == id);
        if (item == null)
        {
            Console.WriteLine("Item not found.");
            return;
        }
        Console.Write($"Enter new name (leave blank to keep '{item.Name}'): ");
        var name = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(name)) item.Name = name;
        Console.Write($"Enter new due date (yyyy-MM-dd) or leave blank to keep '{(item.DueDate.HasValue ? item.DueDate.Value.ToShortDateString() : "-")}': ");
        var dueInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(dueInput))
        {
            if (DateTime.TryParse(dueInput, out var dt))
                item.DueDate = dt;
            else
            {
                Console.WriteLine("Invalid date format.");
                return;
            }
        }
        Console.Write($"Enter new notes (leave blank to keep current): ");
        var notes = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(notes)) item.Notes = notes;
        SaveToDoList();
        Console.WriteLine("Item updated.");
    }

    static void DeleteItem()
    {
        Console.Write("Enter ID of item to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }
        var item = ToDoList.FirstOrDefault(x => x.ID == id);
        if (item == null)
        {
            Console.WriteLine("Item not found.");
            return;
        }
        ToDoList.Remove(item);
        SaveToDoList();
        Console.WriteLine("Item deleted.");
    }

    static void ToggleComplete()
    {
        Console.Write("Enter ID of item to mark complete/incomplete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }
        var item = ToDoList.FirstOrDefault(x => x.ID == id);
        if (item == null)
        {
            Console.WriteLine("Item not found.");
            return;
        }
        item.CompleteFlag = !item.CompleteFlag;
        SaveToDoList();
        Console.WriteLine($"Item marked as {(item.CompleteFlag ? "complete" : "incomplete")}.");
    }
}
