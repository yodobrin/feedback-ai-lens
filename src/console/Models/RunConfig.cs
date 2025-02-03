namespace ProductLeaders.console.Models;

public class RunConfig
{
    [JsonPropertyName("service")]
    public string Service { get; set; } = string.Empty; // service name (e.g. cosmosdb)
    [JsonPropertyName("input_file")]
    public string InputFile { get; set; } = string.Empty; // Path to the input file

    [JsonPropertyName("data_folder")]
    public string DataFolder { get; set; } = string.Empty; // Path to the input file


    [JsonPropertyName("output_directory")]
    public string OutputDirectory { get; set; } = string.Empty; // Path to the output directory

    [JsonPropertyName("operation")]
    public string Operation { get; set; } = string.Empty; // Operation type: "summary", "clustering", or "sub-clustering"

    [JsonPropertyName("parameters")]
    public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>(); // Optional parameters like thresholds
}
