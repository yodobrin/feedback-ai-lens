using System.Text.Json;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.ML.Data;
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

    [JsonPropertyName("SubClusters")]
    public List<List<FeedbackRecord>> SubClusters { get; set; } // Array of subclusters

    [JsonPropertyName("Summary")]
    public string Summary { get; set; }
}

public class EmbeddingData
{
    [VectorType(1536)] // Changed the vector size to 1536
    public float[] Embedding { get; set; }
}

// Display the top 5 clusters
public class ClusterPrediction
{
    [ColumnName("PredictedLabel")]
    public uint PredictedCluster { get; set; }
    [ColumnName("Score")]
    public float[] Distances { get; set; }
}
