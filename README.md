# 📋 ToDo CLI Application

A feature-rich command-line to-do list application built with C# .NET 8, featuring email notifications via SendGrid integration and comprehensive testing.

## ✨ Features

- **Task Management**: Create, edit, delete, and mark tasks as complete
- **Data Persistence**: Automatic JSON file storage with crash-safe data handling
- **Email Integration**: SendGrid-powered email notifications and task summaries
- **Rich Display**: Color-coded table output with task status indicators
- **Cross-Platform**: Runs on Windows, macOS, and Linux
- **Comprehensive Testing**: Edge case testing with MSTest framework

## 🚀 Quick Start

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Installation & Setup
```bash
# Clone the repository
git clone <repository-url>
cd Week2CLISprint

# Build the application
cd ToDoCliApp
dotnet build

# Run the application
dotnet run
```

### First Run
The application will automatically create a data directory at:
- **Windows**: `%APPDATA%\ToDoCliApp\todo.json`
- **macOS/Linux**: `~/.config/ToDoCliApp/todo.json`

## 🎯 Usage

### Main Menu Options
1. **List all to-do items** - View all tasks with status indicators
2. **Add a new to-do item** - Create tasks with optional due dates
3. **Edit a to-do item** - Modify existing task details
4. **Delete a to-do item** - Remove tasks permanently
5. **Mark as complete/incomplete** - Toggle task completion status
6. **Email Configuration** - Set up SendGrid integration
7. **Send Task Reminder Email** - Email specific task reminders
8. **Send To-Do List Summary Email** - Email complete task summary
9. **Exit** - Save and quit the application

### Email Setup (Optional)
To enable email features:

1. Get a [SendGrid API key](https://sendgrid.com/docs/ui/account-and-settings/api-keys/)
2. Choose option 6 (Email Configuration) from the main menu
3. Enter your SendGrid API key and email addresses
4. Configuration is saved to `email-config.json`

## 📁 Project Structure

```
Week2CLISprint/
├── ToDoCliApp/                 # Main application
│   ├── Program.cs             # Main program logic
│   ├── ToDoItem.cs            # Task data model
│   ├── EmailService.cs        # SendGrid email integration
│   ├── EmailConfig.cs         # Email configuration model
│   └── ToDoCliApp.csproj      # Project dependencies
├── ToDoCliApp.Tests/          # Test suite
│   ├── ToDoItemEdgeCaseTests.cs # Comprehensive edge case tests
│   └── ToDoCliApp.Tests.csproj  # Test project dependencies
└── README.md                  # This file
```

## 🧪 Testing

Run the comprehensive test suite:

```bash
cd ToDoCliApp.Tests
dotnet test
```

The test suite includes:
- **Extreme Input Testing**: Very long strings, special characters, Unicode
- **Edge Case Scenarios**: ID conflicts, concurrent access, data corruption
- **Data Persistence Testing**: JSON serialization, file recovery, large datasets

## 🔧 Technical Details

- **Framework**: .NET 8.0
- **Language**: C# with nullable reference types
- **Data Storage**: JSON files using System.Text.Json
- **Email Service**: SendGrid API integration
- **Testing**: MSTest framework with edge case coverage
- **Platform**: Cross-platform console application

## 📊 Example Output

```
| # | Name              | Created      | Due        | Done | Notes                    |
|---|-------------------|--------------|------------|------|--------------------------|
| 1 | Create CLI App    | 2025-07-09   | 2025-07-10 | [x]  |                          |
| 2 | Add SendGrid      | 2025-07-09   | -          | [ ]  | Email integration        |
| 3 | Write Tests       | 2025-07-10   | 2025-07-11 | [x]  | Edge case testing        |
```

## 🛡️ Data Safety

- **Automatic Backups**: Previous data is preserved during updates
- **Error Recovery**: Graceful handling of corrupted files
- **Consistent Storage**: Data location doesn't change based on run directory
- **Safe Serialization**: Robust JSON handling with error recovery

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make your changes
4. Run tests (`dotnet test`)
5. Commit your changes (`git commit -m 'Add amazing feature'`)
6. Push to the branch (`git push origin feature/amazing-feature`)
7. Open a Pull Request

## 📧 Email Features

### Task Reminders
Send individual task reminders with:
- Task details and due dates
- Priority indicators
- Custom message content

### Summary Reports
Generate comprehensive email reports including:
- All pending tasks
- Completed tasks summary
- Overdue task alerts
- Daily/weekly summaries

## 🔍 Troubleshooting

### Common Issues

**Data disappeared after update?**
- Check `%APPDATA%\ToDoCliApp\` for your data file
- See `DATA_RECOVERY_EXPLANATION.md` for recovery steps

**Email not sending?**
- Verify SendGrid API key is valid
- Check email configuration in option 6
- Ensure sender email is verified in SendGrid

**Build errors?**
- Ensure .NET 8.0 SDK is installed
- Run `dotnet restore` to restore packages
- Check that test files are in `ToDoCliApp.Tests/` directory

## 📝 License

This project is part of an AI programming course sprint exercise.

---

**Built with ❤️ using .NET 8 and SendGrid**
