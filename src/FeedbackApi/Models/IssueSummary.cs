using System.Text.Json.Serialization;

// The model for the issue summarypublic class IssueSummary
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

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("summary")]
    public SummaryDetail Summary { get; set; }
}

public class SummaryDetail
{
    [JsonPropertyName("main_points")]
    public List<MainPoint> MainPoints { get; set; }
}

public class MainPoint
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("description")]
    public List<string> Description { get; set; }
}