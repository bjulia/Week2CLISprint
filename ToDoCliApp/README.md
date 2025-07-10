# To-Do CLI App with SendGrid Email Integration

A command-line to-do list application with SendGrid email integration for sending task reminders and summaries.

## Features

- ✅ Create, read, update, and delete to-do items
- ✅ Mark items as complete/incomplete
- ✅ JSON-based data persistence
- ✅ **NEW**: Send email reminders for specific tasks
- ✅ **NEW**: Send email summaries of your entire to-do list
- ✅ **NEW**: Beautiful HTML email templates

## Prerequisites

- .NET 8.0 SDK
- SendGrid account and API key

## Setup

1. **Clone/Download the project**
   ```bash
   git clone [your-repo-url]
   cd ToDoCliApp
   ```

2. **Install dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the project**
   ```bash
   dotnet build
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

## SendGrid Email Configuration

### Getting Your SendGrid API Key

1. Sign up for a free SendGrid account at https://sendgrid.com/
2. Navigate to Settings > API Keys
3. Create a new API key with "Full Access" permissions
4. Copy the API key (you'll need this for configuration)

### Configuring Email in the App

1. Run the application: `dotnet run`
2. Select option **6** (Email Configuration)
3. Enter the following information:
   - **SendGrid API Key**: Your SendGrid API key from above
   - **From Email**: The email address you want to send from (must be verified in SendGrid)
   - **From Name**: The display name for the sender
   - **Default To Email**: Your email address (where you want to receive notifications)
   - **Default To Name**: Your name

### Email Domain Verification

For production use, you'll need to verify your sending domain in SendGrid:
1. Go to Settings > Sender Authentication in SendGrid
2. Verify your domain or single sender email address
3. For testing, you can use the single sender verification

## Usage

### Basic To-Do Operations

1. **List Items**: View all your to-do items in a formatted table
2. **Add Item**: Create a new to-do item with name, due date, and notes
3. **Edit Item**: Modify existing to-do items
4. **Delete Item**: Remove to-do items
5. **Mark Complete/Incomplete**: Toggle completion status

### Email Features

6. **Email Configuration**: Set up SendGrid settings
7. **Send Task Reminder Email**: Send a reminder email for a specific task
8. **Send To-Do List Summary Email**: Send a complete summary of all your tasks

### Email Templates

The application includes two types of emails:

#### Task Reminder Email
- Sends details about a specific task
- Includes task name, creation date, due date, status, and notes
- Available in both plain text and HTML formats

#### Summary Email
- Sends an overview of your entire to-do list
- Shows total tasks, incomplete tasks, and completed tasks
- Lists all incomplete tasks with due dates
- Lists all completed tasks
- Beautiful HTML formatting with color-coded sections

## File Structure

```
ToDoCliApp/
├── Program.cs          # Main application logic
├── ToDoItem.cs         # To-do item model
├── EmailService.cs     # SendGrid email functionality
├── EmailConfig.cs      # Email configuration management
├── ToDoCliApp.csproj   # Project file
├── todo.json          # Data storage (created automatically)
└── email-config.json  # Email settings (created automatically)
```

## Configuration Files

### todo.json
Stores your to-do items in JSON format. Created automatically when you add your first item.

### email-config.json
Stores your SendGrid configuration. Created when you configure email settings (option 6).

**⚠️ Important**: Never commit `email-config.json` to version control as it contains your API key!

## Sample Usage

1. **Add a task**:
   ```
   Select option: 2
   Enter name: Buy groceries
   Enter due date: 2025-07-15
   Enter notes: Milk, bread, eggs
   ```

2. **Send task reminder**:
   ```
   Select option: 7
   Enter ID of task to send reminder for: 1
   Send to email: [press Enter for default]
   Send to name: [press Enter for default]
   ```

3. **Send summary email**:
   ```
   Select option: 8
   Send to email: [press Enter for default]
   Send to name: [press Enter for default]
   ```

## Troubleshooting

### Email Not Sending

1. **Check API Key**: Ensure your SendGrid API key is correct
2. **Verify Sender**: Make sure your "From Email" is verified in SendGrid
3. **Check Quota**: Free SendGrid accounts have daily limits
4. **Firewall**: Ensure your network allows HTTPS connections to SendGrid

### Build Errors

1. **Missing Package**: Run `dotnet restore` to install dependencies
2. **Wrong .NET Version**: Ensure you have .NET 8.0 SDK installed

## Security Notes

- The email configuration file contains your API key
- Never share or commit the `email-config.json` file
- Use environment variables for production deployments
- Consider using restricted API keys with minimal permissions

## Future Enhancements

- [ ] Scheduled email reminders
- [ ] Multiple email recipients
- [ ] Email templates customization
- [ ] Integration with calendar systems
- [ ] Email attachments with task exports

## License

This project is open source and available under the MIT License.
