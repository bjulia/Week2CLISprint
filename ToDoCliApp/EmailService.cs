using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

public class EmailService
{
    private readonly string _apiKey;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public EmailService(string apiKey, string fromEmail, string fromName)
    {
        _apiKey = apiKey;
        _fromEmail = fromEmail;
        _fromName = fromName;
    }

    public async Task<bool> SendToDoReminderAsync(string toEmail, string toName, ToDoItem todoItem)
    {
        try
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(toEmail, toName);
            var subject = $"To-Do Reminder: {todoItem.Name}";
            
            var plainTextContent = CreatePlainTextContent(todoItem);
            var htmlContent = CreateHtmlContent(todoItem);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            
            var response = await client.SendEmailAsync(msg);
            
            return response.StatusCode == System.Net.HttpStatusCode.Accepted;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SendToDoListSummaryAsync(string toEmail, string toName, List<ToDoItem> todoItems)
    {
        try
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(toEmail, toName);
            var subject = "Your To-Do List Summary";
            
            var plainTextContent = CreateSummaryPlainTextContent(todoItems);
            var htmlContent = CreateSummaryHtmlContent(todoItems);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            
            var response = await client.SendEmailAsync(msg);
            
            return response.StatusCode == System.Net.HttpStatusCode.Accepted;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            return false;
        }
    }

    private string CreatePlainTextContent(ToDoItem todoItem)
    {
        var content = $"Hi!\n\nThis is a reminder about your to-do item:\n\n";
        content += $"Task: {todoItem.Name}\n";
        content += $"Created: {todoItem.CreateDate:yyyy-MM-dd}\n";
        content += $"Due Date: {(todoItem.DueDate?.ToString("yyyy-MM-dd") ?? "Not set")}\n";
        content += $"Status: {(todoItem.CompleteFlag ? "Complete" : "Incomplete")}\n";
        
        if (!string.IsNullOrWhiteSpace(todoItem.Notes))
        {
            content += $"Notes: {todoItem.Notes}\n";
        }
        
        content += "\n---\nSent from To-Do CLI App";
        return content;
    }

    private string CreateHtmlContent(ToDoItem todoItem)
    {
        var statusColor = todoItem.CompleteFlag ? "green" : "orange";
        var statusText = todoItem.CompleteFlag ? "✓ Complete" : "⏳ Incomplete";
        
        var html = $@"
        <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #2c3e50; border-bottom: 3px solid #3498db; padding-bottom: 10px;'>
                        📝 To-Do Reminder
                    </h2>
                    <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0;'>
                        <h3 style='color: #2c3e50; margin-top: 0;'>{todoItem.Name}</h3>
                        <p><strong>Created:</strong> {todoItem.CreateDate:yyyy-MM-dd}</p>
                        <p><strong>Due Date:</strong> {(todoItem.DueDate?.ToString("yyyy-MM-dd") ?? "Not set")}</p>
                        <p><strong>Status:</strong> <span style='color: {statusColor}; font-weight: bold;'>{statusText}</span></p>";
        
        if (!string.IsNullOrWhiteSpace(todoItem.Notes))
        {
            html += $"<p><strong>Notes:</strong> {todoItem.Notes}</p>";
        }
        
        html += @"
                    </div>
                    <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'>
                    <p style='color: #666; font-size: 12px; text-align: center;'>
                        Sent from To-Do CLI App
                    </p>
                </div>
            </body>
        </html>";
        
        return html;
    }

    private string CreateSummaryPlainTextContent(List<ToDoItem> todoItems)
    {
        var content = $"Hi!\n\nHere's your to-do list summary:\n\n";
        
        var incomplete = todoItems.Where(t => !t.CompleteFlag).ToList();
        var complete = todoItems.Where(t => t.CompleteFlag).ToList();
        
        content += $"Total Tasks: {todoItems.Count}\n";
        content += $"Incomplete: {incomplete.Count}\n";
        content += $"Complete: {complete.Count}\n\n";
        
        if (incomplete.Any())
        {
            content += "INCOMPLETE TASKS:\n";
            content += "==================\n";
            foreach (var item in incomplete)
            {
                content += $"• {item.Name}";
                if (item.DueDate.HasValue)
                {
                    content += $" (Due: {item.DueDate.Value:yyyy-MM-dd})";
                }
                content += "\n";
            }
            content += "\n";
        }
        
        if (complete.Any())
        {
            content += "COMPLETED TASKS:\n";
            content += "================\n";
            foreach (var item in complete)
            {
                content += $"✓ {item.Name}\n";
            }
        }
        
        content += "\n---\nSent from To-Do CLI App";
        return content;
    }

    private string CreateSummaryHtmlContent(List<ToDoItem> todoItems)
    {
        var incomplete = todoItems.Where(t => !t.CompleteFlag).ToList();
        var complete = todoItems.Where(t => t.CompleteFlag).ToList();
        
        var html = $@"
        <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #2c3e50; border-bottom: 3px solid #3498db; padding-bottom: 10px;'>
                        📊 To-Do List Summary
                    </h2>
                    <div style='display: flex; gap: 20px; margin: 20px 0;'>
                        <div style='background-color: #f8f9fa; padding: 15px; border-radius: 8px; text-align: center; flex: 1;'>
                            <h3 style='margin: 0; color: #2c3e50;'>{todoItems.Count}</h3>
                            <p style='margin: 5px 0; color: #666;'>Total Tasks</p>
                        </div>
                        <div style='background-color: #fff3cd; padding: 15px; border-radius: 8px; text-align: center; flex: 1;'>
                            <h3 style='margin: 0; color: #856404;'>{incomplete.Count}</h3>
                            <p style='margin: 5px 0; color: #666;'>Incomplete</p>
                        </div>
                        <div style='background-color: #d1edff; padding: 15px; border-radius: 8px; text-align: center; flex: 1;'>
                            <h3 style='margin: 0; color: #0c5460;'>{complete.Count}</h3>
                            <p style='margin: 5px 0; color: #666;'>Complete</p>
                        </div>
                    </div>";
        
        if (incomplete.Any())
        {
            html += @"
                    <div style='margin: 30px 0;'>
                        <h3 style='color: #856404; margin-bottom: 15px;'>⏳ Incomplete Tasks</h3>
                        <ul style='list-style: none; padding: 0;'>";
            
            foreach (var item in incomplete)
            {
                html += $@"
                            <li style='background-color: #fff3cd; padding: 10px; margin: 5px 0; border-radius: 5px; border-left: 4px solid #ffc107;'>
                                <strong>{item.Name}</strong>";
                if (item.DueDate.HasValue)
                {
                    html += $" <span style='color: #856404; font-size: 12px;'>(Due: {item.DueDate.Value:yyyy-MM-dd})</span>";
                }
                html += "</li>";
            }
            
            html += @"
                        </ul>
                    </div>";
        }
        
        if (complete.Any())
        {
            html += @"
                    <div style='margin: 30px 0;'>
                        <h3 style='color: #0c5460; margin-bottom: 15px;'>✅ Completed Tasks</h3>
                        <ul style='list-style: none; padding: 0;'>";
            
            foreach (var item in complete)
            {
                html += $@"
                            <li style='background-color: #d1edff; padding: 10px; margin: 5px 0; border-radius: 5px; border-left: 4px solid #0dcaf0;'>
                                <strong>{item.Name}</strong>
                            </li>";
            }
            
            html += @"
                        </ul>
                    </div>";
        }
        
        html += @"
                    <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'>
                    <p style='color: #666; font-size: 12px; text-align: center;'>
                        Sent from To-Do CLI App
                    </p>
                </div>
            </body>
        </html>";
        
        return html;
    }
}
