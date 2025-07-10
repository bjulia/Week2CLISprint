using System.Text.Json;

public class EmailConfig
{
    public string ApiKey { get; set; } = "";
    public string FromEmail { get; set; } = "";
    public string FromName { get; set; } = "";
    public string DefaultToEmail { get; set; } = "";
    public string DefaultToName { get; set; } = "";

    private const string ConfigFileName = "email-config.json";

    public static EmailConfig Load()
    {
        if (File.Exists(ConfigFileName))
        {
            try
            {
                var json = File.ReadAllText(ConfigFileName);
                return JsonSerializer.Deserialize<EmailConfig>(json) ?? new EmailConfig();
            }
            catch
            {
                Console.WriteLine("Error loading email configuration. Using defaults.");
                return new EmailConfig();
            }
        }
        return new EmailConfig();
    }

    public void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigFileName, json);
        }
        catch
        {
            Console.WriteLine("Error saving email configuration.");
        }
    }

    public bool IsConfigured()
    {
        return !string.IsNullOrWhiteSpace(ApiKey) && 
               !string.IsNullOrWhiteSpace(FromEmail) && 
               !string.IsNullOrWhiteSpace(DefaultToEmail);
    }
}
