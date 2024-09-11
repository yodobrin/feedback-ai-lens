using System.Text.Json.Serialization;

// The model for the issue summary
public class IssueSummary
{
    [JsonPropertyName("issue")]
    public string Issue { get; set; }

    [JsonPropertyName("similar_issues")]
    public int SimilarIssues { get; set; }

    [JsonPropertyName("distinct_customers")]
    public int DistinctCustomers { get; set; }

    [JsonPropertyName("feedback_links")]
    public List<string> FeedbackLinks { get; set; }

    [JsonPropertyName("summary")]
    public string Summary { get; set; }
}