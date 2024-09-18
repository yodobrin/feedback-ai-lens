using System.Text.Json;  // For JSON serialization/deserialization
using System.Text.Json.Serialization;

public class FeedbackTypeSummary
{
    public string Type { get; set; } = string.Empty;
    public int Count { get; set; }
    public List<FeedbackTypeDetail> Details { get; set; } = new List<FeedbackTypeDetail>();  // List for collapsed categories
}

public class FeedbackTypeDetail
{
    public string OriginalType { get; set; } = string.Empty;
    public int Count { get; set; }
}
public class ServiceHighlight
{
    [JsonPropertyName("ServiceName")]
    public string ServiceName { get; set; } = string.Empty;

    [JsonPropertyName("TotalFeedback")]
    public int TotalFeedback { get; set; }

    [JsonPropertyName("DistinctCustomers")]
    public int DistinctCustomers { get; set; }

    [JsonPropertyName("FeatureRequests")]
    public int FeatureRequests { get; set; }

    [JsonPropertyName("Bugs")]
    public int Bugs { get; set; }

    [JsonPropertyName("OverallSentiment")]
    public string OverallSentiment { get; set; } = string.Empty;

    public List<FeedbackTypeSummary> FeedbackTypes { get; set; } = new List<FeedbackTypeSummary>();
}
