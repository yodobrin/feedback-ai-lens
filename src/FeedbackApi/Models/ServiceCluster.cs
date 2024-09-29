
public class ServiceCluster
{
    [JsonPropertyName("ClusterId")]
    public string ClusterId { get; set; } = string.Empty;

    [JsonPropertyName("CommonElement")]
    public string CommonElement { get; set; } = string.Empty;

    [JsonPropertyName("SimilarFeedbacks")]
    public int SimilarFeedbacks { get; set; }

    [JsonPropertyName("DistinctCustomers")]
    public int DistinctCustomers { get; set; }

    [JsonPropertyName("FeedbackRecords")]
    public List<FeedbackRecord> FeedbackRecords { get; set; } = new List<FeedbackRecord>();

    [JsonPropertyName("Summary")]
    public string Summary { get; set; } = string.Empty;
}
