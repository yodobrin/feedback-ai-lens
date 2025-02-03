
public class ClusterUtility
{

    public static List<(FeedbackRecord Feedback, uint Cluster)> CreateClusters(List<FeedbackRecord> feedbackRecords, int clusterCount )
    {
        // var clusterCount = 50;
        var mlContext = new MLContext();
        var embeddingData = feedbackRecords.Select(f => new EmbeddingData { Embedding = f.Embedding }).ToList();
        var dataView = mlContext.Data.LoadFromEnumerable(embeddingData);

        // Cluster the embeddings using KMeans (set number of clusters, e.g., 5)
        var pipeline = mlContext.Clustering.Trainers.KMeans(featureColumnName: "Embedding", numberOfClusters: clusterCount);
        var model = pipeline.Fit(dataView);

        // Predict the cluster for each feedback record
        var predictions = model.Transform(dataView);
        var clusters = mlContext.Data.CreateEnumerable<ClusterPrediction>(predictions, reuseRowObject: false).ToList();

        // Console.WriteLine($"Number of clusters: {clusters.Count}");



        List<(FeedbackRecord Feedback, uint Cluster)> feedbackWithClusters = feedbackRecords
            .Zip(clusters, (feedback, cluster) =>
                (Feedback: feedback, Cluster: cluster.PredictedCluster)
            )
            .ToList();



        // print the number of numberOfClusters
        var numberOfClusters = feedbackWithClusters.Select(f => f.Cluster).Distinct().Count();
        Console.WriteLine($"Number of clusters: {numberOfClusters}");
        return feedbackWithClusters;
    }
    public static List<ServiceCluster> GenerateClusters(List<(FeedbackRecord Feedback, uint Cluster)> feedbackWithClusters)
    {
        var serviceClusters = feedbackWithClusters
            .GroupBy(fc => fc.Cluster) // Group by the predicted cluster
            .Select(clusterGroup =>
            {
                // Collect full FeedbackRecords for this cluster
                var feedbackRecords = clusterGroup
                    .Select(fc => fc.Feedback)
                    .ToList();

                // Calculate distinct customers
                var distinctCustomers = feedbackRecords
                    .Select(f => f.CustomerName)
                    .Distinct()
                    .Count();

                // Create the service cluster object
                return new ServiceCluster
                {
                    ClusterId = clusterGroup.Key.ToString(CultureInfo.InvariantCulture),
                    CommonElement = "Common Theme Placeholder", // Replace with actual summarization from OpenAI
                    SimilarFeedbacks = feedbackRecords.Count,
                    DistinctCustomers = distinctCustomers,
                    FeedbackRecords = feedbackRecords,  // Full feedback records
                    Summary = "Cluster summary placeholder" // Use OpenAI for summarization
                };
            })
            .ToList();

        return serviceClusters;
    }


}




public class ClusterPrediction
{
    [ColumnName("PredictedLabel")]
    public uint PredictedCluster { get; set; }  // Cluster number (1, 2, 3, etc.)
}
