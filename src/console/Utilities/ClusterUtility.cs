namespace ProductLeaders.console.Utilities;
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

    public static async Task<List<ServiceCluster>> GenerateClustersUsingLLM(
        List<FeedbackRecord> feedbackRecords,
        OpenAIClient openAIClient,
        string chatCompletionDeploymentName,
        string systemMessage)
    {
        // Build a prompt that includes all feedback items.
        // create a string that has the user story and the id of the feedback without using stringbuilder

         // Create a collection of strings for each feedback record.
        var feedbackTexts = feedbackRecords.Select(feedback =>
            $"Id: {feedback.Id}\nUserStory: {feedback.UserStory}");
        string prompt = string.Join("\n\n", feedbackTexts);
Console.WriteLine($"Prompt: {prompt}");
        // Use EnrichUtility to get ChatCompletionsOptions for clusters.
        // (Ensure that GetOptions4Clusters is public or accessible in your project.)
        ChatCompletionsOptions options = EnrichUtility.GetOptions4Clusters(prompt, systemMessage, chatCompletionDeploymentName);
Console.WriteLine("calling openai");
        // Call the LLM using EnrichUtility. The LLM is expected to return a JSON array.
        string llmResponse = await EnrichUtility.CallOpenAI(openAIClient, options, "No response generated.");

        // Deserialize the JSON response into a list of classification objects.
        List<LLMClassificationResponse>? classifications = null;
        try
        {
            classifications = JsonSerializer.Deserialize<List<LLMClassificationResponse>>(llmResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deserializing LLM response: {ex.Message}");
            return new List<ServiceCluster>();
        }

        if (classifications == null)
        {
            Console.WriteLine("LLM returned null classifications.");
            return new List<ServiceCluster>();
        }

        // Create ServiceCluster objects based on the LLM classifications.
        List<ServiceCluster> clusters = new List<ServiceCluster>();
        foreach (var classification in classifications)
        {
            // Find the feedback records that match the returned feedback IDs.

            var clusterFeedbackRecords = feedbackRecords
                .Where(f => classification.FeedbackIds.Contains(f.Id))
                .ToList();
            Console.WriteLine($"Creating cluster for common element: {classification.CommonElement} with {clusterFeedbackRecords.Count} feedback records.");
            clusters.Add(new ServiceCluster
            {
                ClusterId = classification.CommonElement, // Alternatively, generate a unique ID if needed.
                CommonElement = classification.CommonElement,
                FeedbackRecords = clusterFeedbackRecords,
                SimilarFeedbacks = clusterFeedbackRecords.Count,
                DistinctCustomers = clusterFeedbackRecords.Select(f => f.CustomerName).Distinct().Count(),
                Summary = string.Empty // Summary can be added later if desired.
            });
        }

        return clusters;
    }

    public static async Task<List<ServiceCluster>> GenerateClustersUsingLLMBatched(
        List<FeedbackRecord> feedbackRecords,
        OpenAIClient openAIClient,
        string chatCompletionDeploymentName,
        int batchSize = 50)
    {
        // Initialize an empty classification
        List<LLMClassificationResponse> currentClassification = new List<LLMClassificationResponse>();

        // Partition the feedbackRecords into batches
        var batches = feedbackRecords
            .Select((record, index) => new { record, index })
            .GroupBy(x => x.index / batchSize)
            .Select(g => g.Select(x => x.record).ToList())
            .ToList();

        // Process each batch sequentially
        var iteration = 0;
        foreach (var batch in batches)
        {
            // Build a dynamic system message by appending the current classification if available.
            string dynamicSystemMessage = IOpenAIConstants.CreateClusterByLLMSystemMessage;
            if (currentClassification.Any())
            {
                dynamicSystemMessage += "\n\nCurrent Classification:\n" +
                    JsonSerializer.Serialize(currentClassification, new JsonSerializerOptions { WriteIndented = false });
        //go over the current classification and print just the common element and the number of feedback ids

                Console.WriteLine($"Current {iteration++} iteration classifications:");
                foreach (var classification in currentClassification)
                {
                    Console.WriteLine($" {classification.CommonElement} with {classification.FeedbackIds.Count} feedback ids.");
                 }
            }


            // Build the prompt using LINQ's Select to list the new feedback items.
            var feedbackTexts = batch.Select(feedback =>
                $"Id: {feedback.Id}\nUserStory: {feedback.UserStory}");
            string prompt = string.Join("\n\n", feedbackTexts);

            // Get ChatCompletionsOptions using EnrichUtility
            ChatCompletionsOptions options = EnrichUtility.GetOptions4Clusters(prompt, dynamicSystemMessage, chatCompletionDeploymentName);

            // Call the LLM
            string llmResponse = await EnrichUtility.CallOpenAI(openAIClient, options, "No response generated.");

            // Deserialize the response into a wrapper object that contains the list of clusters
            LLMClusterResponseWrapper? wrapper = null;
            try
            {
                wrapper = JsonSerializer.Deserialize<LLMClusterResponseWrapper>(llmResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deserializing LLM response: {ex.Message}");
                continue; // Optionally, decide to break or retry.
            }

            if (wrapper != null)
            {
                // Update current classification with the new clusters returned by the LLM.
                currentClassification = wrapper.Clusters;
            }
        }


        // Build ServiceCluster objects from the final classification.
        List<ServiceCluster> clusters = new List<ServiceCluster>();
        foreach (var classification in currentClassification)
        {
            var clusterFeedbackRecords = feedbackRecords
                .Where(f => classification.FeedbackIds.Contains(f.Id))
                .ToList();

            clusters.Add(new ServiceCluster
            {
                ClusterId = classification.CommonElement, // You may generate a unique ID if preferred.
                CommonElement = classification.CommonElement,
                FeedbackRecords = clusterFeedbackRecords,
                SimilarFeedbacks = clusterFeedbackRecords.Count,
                DistinctCustomers = clusterFeedbackRecords.Select(f => f.CustomerName).Distinct().Count(),
                Summary = string.Empty // Optionally, you can update this later.
            });
        }

        return clusters;
    }


}




public class ClusterPrediction
{
    [ColumnName("PredictedLabel")]
    public uint PredictedCluster { get; set; }  // Cluster number (1, 2, 3, etc.)
}

/// <summary>
/// Represents the expected structure of each classification returned by the LLM.
/// </summary>
public class LLMClassificationResponse
{
    [JsonPropertyName("CommonElement")]
    public string CommonElement { get; set; } = string.Empty;

    [JsonPropertyName("FeedbackIds")]
    public List<string> FeedbackIds { get; set; } = new List<string>();
}

public class LLMClusterResponseWrapper
{
    [JsonPropertyName("clusters")]
    public List<LLMClassificationResponse> Clusters { get; set; } = new List<LLMClassificationResponse>();
}
