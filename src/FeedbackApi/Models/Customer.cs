using System.Text.Json.Serialization;

public class Customer
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("industry")]
    public string Industry { get; set; }

    [JsonPropertyName("tpid")]
    public string Tpid { get; set; }
    [JsonPropertyName("feedback_title")]
    public string FeedbackTitle { get; set; }
}