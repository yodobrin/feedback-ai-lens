

public class ServiceCluster
{
    [JsonPropertyName("ServiceName")]
    public string ServiceName { get; set; }

    [JsonPropertyName("ClusterName")]
    public string ClusterName { get; set; }

    [JsonPropertyName("CommonElement")]
    public string CommonElement { get; set; }

    [JsonPropertyName("SimilarFeedbacks")]
    public int SimilarFeedbacks { get; set; }

    [JsonPropertyName("DistinctCustomers")]
    public int DistinctCustomers { get; set; }
}