
public class ServiceHighlight
{
    [JsonPropertyName("ServiceName")]
    public string ServiceName { get; set; }

    [JsonPropertyName("TotalFeedback")]
    public int TotalFeedback { get; set; }

    [JsonPropertyName("DistinctCustomers")]
    public int DistinctCustomers { get; set; }

    [JsonPropertyName("FeatureRequests")]
    public int FeatureRequests { get; set; }

    [JsonPropertyName("Bugs")]
    public int Bugs { get; set; }

    [JsonPropertyName("OverallSentiment")]
    public string OverallSentiment { get; set; }
}