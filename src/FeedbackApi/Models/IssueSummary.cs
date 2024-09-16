using System.Text.Json.Serialization;

// The model for the issue summarypublic class IssueSummary
public class IssueSummary
{
    [JsonPropertyName("issue")]
    public string Issue { get; set; } = string.Empty;

    [JsonPropertyName("similar_issues")]
    public int SimilarIssues { get; set; }

    [JsonPropertyName("distinct_customers")]
    public int DistinctCustomers { get; set; }

    [JsonPropertyName("feedback_links")]
    public List<string> FeedbackLinks { get; set; } = new List<string>();

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("summary")]
    public SummaryDetail Summary { get; set; } = new SummaryDetail();
}

public class SummaryDetail
{
    [JsonPropertyName("main_points")]
    public List<MainPoint> MainPoints { get; set; } = new List<MainPoint>();
}

public class MainPoint
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public List<string> Description { get; set; } = new List<string>();
}