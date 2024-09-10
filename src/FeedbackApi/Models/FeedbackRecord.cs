using System.Text.Json.Serialization;
public class FeedbackRecord
{
    [JsonPropertyName("Id")]
    public string Id { get; set; }

    [JsonPropertyName("PartnerShortName")]
    public string PartnerShortName { get; set; }

    [JsonPropertyName("ServiceName")]
    public string ServiceName { get; set; }

    [JsonPropertyName("Type")]
    public string Type { get; set; }

    [JsonPropertyName("Title")]
    public string Title { get; set; }

    [JsonPropertyName("Blocking")]
    public string Blocking { get; set; }

    [JsonPropertyName("Description")]
    public string Description { get; set; }

    [JsonPropertyName("WorkaroundAvailable")]
    public string WorkaroundAvailable { get; set; }

    [JsonPropertyName("Priority")]
    public string Priority { get; set; }

    [JsonPropertyName("CustomerName")]
    public string CustomerName { get; set; }

    [JsonPropertyName("CustomerTpid")]
    public string CustomerTpid { get; set; }

    [JsonPropertyName("WorkaroundDescription")]
    public string WorkaroundDescription { get; set; }

    [JsonPropertyName("UserStory")]
    public string UserStory { get; set; }

    [JsonPropertyName("Embedding")]
    public float[] Embedding { get; set; } // Embedding for the user story as a float array
}