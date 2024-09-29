using System.Text.Json;  // For JSON serialization/deserialization
using System.Text.Json.Serialization;

public class ServiceMappingConfig
{
    [JsonPropertyName("serviceMappings")]  // Necessary for System.Text.Json if JSON and property name differ
    public List<ServiceDescriptor> Services { get; set; } = new List<ServiceDescriptor>();
    // public Dictionary<string, string> ServiceMappings { get; set; } = new Dictionary<string, string>();
}

public class ServiceDescriptor
{
    [JsonPropertyName("marketingName")]
    public string MarketingName { get; set; } = string.Empty;

    [JsonPropertyName("internalId")]
    public string InternalId { get; set;} = string.Empty;
    [JsonPropertyName("filePatterns")]
    public FilePatterns FilePatterns { get; set; } = new FilePatterns();
}

public class FilePatterns
{
    [JsonPropertyName("vectorFile")]
    public string Vector { get; set; } = string.Empty;
    [JsonPropertyName("csvFile")]
    public string Csv { get; set; } = string.Empty;
    [JsonPropertyName("clusterFile")]
    public string Clusters { get; set; } = string.Empty;

}
