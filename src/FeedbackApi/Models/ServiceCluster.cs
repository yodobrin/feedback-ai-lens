

public class ServiceCluster
{
    [JsonPropertyName("ClusterId")]
    public string ClusterId { get; set; }

    [JsonPropertyName("CommonElement")]
    public string CommonElement { get; set; }

    [JsonPropertyName("SimilarFeedbacks")]
    public int SimilarFeedbacks { get; set; }

    [JsonPropertyName("DistinctCustomers")]
    public int DistinctCustomers { get; set; }

    [JsonPropertyName("FeedbackRecords")]
    public List<FeedbackRecord> FeedbackRecords { get; set; } // Array of full FeedbackRecords

    [JsonPropertyName("Summary")]
    public string Summary { get; set; }
}