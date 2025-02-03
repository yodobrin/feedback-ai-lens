namespace ProductLeaders.console.Models;
public class FeedbackRecord
{
    [JsonPropertyName("Id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("PartnerShortName")]
    public string PartnerShortName { get; set; } = string.Empty;

    [JsonPropertyName("ServiceName")]
    public string ServiceName { get; set; } = string.Empty;

    [JsonPropertyName("Type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("Title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("Blocking")]
    public string Blocking { get; set; } = string.Empty;

    [JsonPropertyName("Description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("WorkaroundAvailable")]
    public string WorkaroundAvailable { get; set; } = string.Empty;

    [JsonPropertyName("Priority")]
    public string Priority { get; set; } = string.Empty;

    [JsonPropertyName("CustomerName")]
    public string CustomerName { get; set; } = string.Empty;

    [JsonPropertyName("CustomerTpid")]
    public string CustomerTpid { get; set; } = string.Empty;

    [JsonPropertyName("WorkaroundDescription")]
    public string WorkaroundDescription { get; set; } = string.Empty;

    [JsonPropertyName("UserStory")]
    public string UserStory { get; set; } = string.Empty;

    [JsonPropertyName("Embedding")]
    public float[]? Embedding { get; set; } = new float[1536];


}
