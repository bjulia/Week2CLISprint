using System.Text.Json;
using System.Text.Json.Serialization;

class Program
{
    // Use absolute path to ensure consistent data location
    static readonly string FileName = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "ToDoCliApp", 
        "todo.json"
    );
    static List<ToDoItem> ToDoList = new();
    static EmailConfig EmailConfiguration = new();
    static EmailService? EmailServiceInstance;

    static async Task Main()
    {
        LoadToDoList();
        LoadEmailConfig();
        
        while (true)
        {
            Console.WriteLine("\nTo-Do List CLI");
            Console.WriteLine("1. List all to-do items");
            Console.WriteLine("2. Add a new to-do item");
            Console.WriteLine("3. Edit a to-do item");
            Console.WriteLine("4. Delete a to-do item");
            Console.WriteLine("5. Mark as complete/incomplete");
            Console.WriteLine("6. Email Configuration");
            Console.WriteLine("7. Send Task Reminder Email");
            Console.WriteLine("8. Send To-Do List Summary Email");
            Console.WriteLine("9. Exit");
            Console.Write("Select an option: ");
            var input = Console.ReadLine();
            switch (input)
            {
                case "1": ListItems(); break;
                case "2": AddItem(); break;
                case "3": EditItem(); break;
                case "4": DeleteItem(); break;
                case "5": ToggleComplete(); break;
                case "6": ConfigureEmail(); break;
                case "7": await SendTaskReminderEmail(); break;
                case "8": await SendSummaryEmail(); break;
                case "9": SaveToDoList(); return;
                default: Console.WriteLine("Invalid option. Try again."); break;
            }
        }
    }

    static void LoadToDoList()
    {
        // Ensure the directory exists
        var directory = Path.GetDirectoryName(FileName);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

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
    
    static void LoadEmailConfig()
    {
        EmailConfiguration = EmailConfig.Load();
        if (EmailConfiguration.IsConfigured())
        {
            EmailServiceInstance = new EmailService(
                EmailConfiguration.ApiKey,
                EmailConfiguration.FromEmail,
                EmailConfiguration.FromName);
        }
    }

    static void ConfigureEmail()
    {
        Console.WriteLine("\n=== Email Configuration ===");
        Console.WriteLine("Enter your SendGrid configuration details:");
        
        Console.Write($"SendGrid API Key (current: {(string.IsNullOrWhiteSpace(EmailConfiguration.ApiKey) ? "not set" : "***hidden***")}): ");
        var apiKey = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            EmailConfiguration.ApiKey = apiKey;
        }
        
        Console.Write($"From Email (current: {EmailConfiguration.FromEmail}): ");
        var fromEmail = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(fromEmail))
        {
            EmailConfiguration.FromEmail = fromEmail;
        }
        
        Console.Write($"From Name (current: {EmailConfiguration.FromName}): ");
        var fromName = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(fromName))
        {
            EmailConfiguration.FromName = fromName;
        }
        
        Console.Write($"Default To Email (current: {EmailConfiguration.DefaultToEmail}): ");
        var toEmail = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(toEmail))
        {
            EmailConfiguration.DefaultToEmail = toEmail;
        }
        
        Console.Write($"Default To Name (current: {EmailConfiguration.DefaultToName}): ");
        var toName = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(toName))
        {
            EmailConfiguration.DefaultToName = toName;
        }
        
        EmailConfiguration.Save();
        LoadEmailConfig(); // Reload to update the service instance
        
        Console.WriteLine("Email configuration saved!");
        
        if (EmailConfiguration.IsConfigured())
        {
            Console.WriteLine("✅ Email functionality is now available.");
        }
        else
        {
            Console.WriteLine("⚠️  Email configuration is incomplete. Please set API Key, From Email, and Default To Email.");
        }
    }

    static async Task SendTaskReminderEmail()
    {
        if (!EmailConfiguration.IsConfigured() || EmailServiceInstance == null)
        {
            Console.WriteLine("Email is not configured. Please configure email settings first (option 6).");
            return;
        }
        
        if (ToDoList.Count == 0)
        {
            Console.WriteLine("No to-do items found.");
            return;
        }
        
        Console.Write("Enter ID of task to send reminder for: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }
        
        var item = ToDoList.FirstOrDefault(x => x.ID == id);
        if (item == null)
        {
            Console.WriteLine("Task not found.");
            return;
        }
        
        Console.Write($"Send to email (press Enter for default: {EmailConfiguration.DefaultToEmail}): ");
        var toEmail = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(toEmail))
        {
            toEmail = EmailConfiguration.DefaultToEmail;
        }
        
        Console.Write($"Send to name (press Enter for default: {EmailConfiguration.DefaultToName}): ");
        var toName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(toName))
        {
            toName = EmailConfiguration.DefaultToName;
        }
        
        Console.WriteLine("Sending email...");
        
        var success = await EmailServiceInstance.SendToDoReminderAsync(toEmail, toName, item);
        
        if (success)
        {
            Console.WriteLine("✅ Email sent successfully!");
        }
        else
        {
            Console.WriteLine("❌ Failed to send email. Please check your configuration and try again.");
        }
    }

    static async Task SendSummaryEmail()
    {
        if (!EmailConfiguration.IsConfigured() || EmailServiceInstance == null)
        {
            Console.WriteLine("Email is not configured. Please configure email settings first (option 6).");
            return;
        }
        
        if (ToDoList.Count == 0)
        {
            Console.WriteLine("No to-do items found.");
            return;
        }
        
        Console.Write($"Send to email (press Enter for default: {EmailConfiguration.DefaultToEmail}): ");
        var toEmail = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(toEmail))
        {
            toEmail = EmailConfiguration.DefaultToEmail;
        }
        
        Console.Write($"Send to name (press Enter for default: {EmailConfiguration.DefaultToName}): ");
        var toName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(toName))
        {
            toName = EmailConfiguration.DefaultToName;
        }
        
        Console.WriteLine("Sending email...");
        
        var success = await EmailServiceInstance.SendToDoListSummaryAsync(toEmail, toName, ToDoList);
        
        if (success)
        {
            Console.WriteLine("✅ Email sent successfully!");
        }
        else
        {
            Console.WriteLine("❌ Failed to send email. Please check your configuration and try again.");
        }
    }
}
