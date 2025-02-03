namespace ProductLeaders.console.Models;
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

    // [JsonPropertyName("SubClusters")]
    // public List<List<FeedbackRecord>> SubClusters { get; set; } // Array of subclusters

    [JsonPropertyName("Summary")]
    public string Summary { get; set; } = string.Empty; // Summary of the cluster
}

public class EmbeddingData
{
    [VectorType(1536)] // Changed the vector size to 1536
    public float[] Embedding { get; set; } = new float[1536];
}

// Display the top 5 clusters
public class ClusterPrediction
{
    [ColumnName("PredictedLabel")]
    public uint PredictedCluster { get; set; }
    [ColumnName("Score")]
    public float[] Distances { get; set; } = new float[1]; // just because
}
public class OpenAIResponse
{
    public string CommonElement { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
}
