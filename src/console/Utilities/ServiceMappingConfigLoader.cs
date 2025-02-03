namespace ProductLeaders.console.Utilities;

public static class ServiceMappingConfigLoader
{
    public static ServiceMappingConfig Load(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Service mapping configuration file not found: {filePath}");
        }

        try
        {
            string jsonContent = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<ServiceMappingConfig>(jsonContent)
                   ?? throw new InvalidOperationException("Failed to deserialize the service mapping configuration.");
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Invalid JSON format in service mapping configuration file.", ex);
        }
    }
}
