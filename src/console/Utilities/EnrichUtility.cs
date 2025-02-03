
namespace ProductLeaders.console.Utilities;


public static class EnrichUtility
{
    // const string Feedback2UserStory =
    //     @"You are an AI assistant. You generate clear generic user stories in text only with the following format:
    //     'As a [persona], I want to [do something], so that I can [achieve something].'
    //     The output should always follow this format without additional styling or formatting.
    //     Do not include specific customers/partner names as part of the output.";
    public static async Task<string> CallOpenAI(
        OpenAIClient openAIClient,
        ChatCompletionsOptions options,
        string fallbackMessage = "No response generated."
    )
    {
        Response<ChatCompletions> response = await openAIClient.GetChatCompletionsAsync(options);
        ChatCompletions completions = response.Value;

        if (completions.Choices.Count > 0)
        {
            return completions.Choices[0].Message.Content;
        }
        else
        {
            return fallbackMessage;
        }
    }

    public static async Task<string> CallOpenAI(string prompt, string systemMessage, OpenAIClient openAIClient, string chatCompletionDeploymentName, string embeddingDeploymentName)
    {
        // Create ChatCompletionsOptions and set up the system and user messages
        ChatCompletionsOptions options = new ChatCompletionsOptions();

        // Add system message
        options.Messages.Add(new ChatRequestSystemMessage(systemMessage));

        // Add user message (the prompt generated from feedback)
        options.Messages.Add(new ChatRequestUserMessage(prompt));

        // Configure request properties
        options.MaxTokens = 500;
        options.Temperature = 0.7f;
        options.NucleusSamplingFactor = 0.95f;
        options.FrequencyPenalty = 0.0f;
        options.PresencePenalty = 0.0f;
        // options.StopSequences.Add("\n");
        options.DeploymentName = chatCompletionDeploymentName;
        options.ResponseFormat = ChatCompletionsResponseFormat.Text;

        // Make the API request to get the chat completions
        Response<ChatCompletions> response = await openAIClient.GetChatCompletionsAsync(options);

        // Extract and return the first response from the choices
        ChatCompletions completions = response.Value;
        if (completions.Choices.Count > 0)
        {
            // output all choices
            // foreach (var choice in completions.Choices)
            // {
            //     Console.WriteLine($"in the loop: {choice.Message.Content}");
            // }
            return completions.Choices[0].Message.Content;
        }
        else
        {
            return "No response generated.";
        }
    }
public static async Task<float[]> GetEmbeddingAsync(string textToBeVecorized, OpenAIClient openAIClient, string embeddingDeploymentName)
{
    // Prepare the embeddings options with the user story
    EmbeddingsOptions embeddingsOptions = new EmbeddingsOptions(embeddingDeploymentName, new List<string> { textToBeVecorized });
    var modelResponse = await openAIClient.GetEmbeddingsAsync( embeddingsOptions);
    float[] response = modelResponse.Value.Data[0].Embedding.ToArray();
    return response;
}

// loop over a csv file, create a json file with user story and embedding
private static ChatCompletionsOptions GetOptions4UserStories(string systemMessage, string prompt, string chatCompletionDeploymentName)
{
    // Create ChatCompletionsOptions and set up the system and user messages
    ChatCompletionsOptions options = new ChatCompletionsOptions();
    // Add system message
    options.Messages.Add(new ChatRequestSystemMessage(systemMessage));
    // Add user message (the prompt generated from feedback)
    options.Messages.Add(new ChatRequestUserMessage(prompt));
    // Configure request properties
    options.MaxTokens = 500;
    options.Temperature = 0.7f;
    options.NucleusSamplingFactor = 0.95f;
    options.FrequencyPenalty = 0.0f;
    options.PresencePenalty = 0.0f;
    // options.StopSequences.Add("\n");
    options.DeploymentName = chatCompletionDeploymentName;
    options.ResponseFormat = ChatCompletionsResponseFormat.Text;
    return options;
}

private static ChatCompletionsOptions GetOptions4Clusters(string prompt, string systemMessage, string chatCompletionDeploymentName)
{
    ChatCompletionsOptions options = new ChatCompletionsOptions();

    // Add system message
    options.Messages.Add(new ChatRequestSystemMessage(systemMessage));

    // Add user message (the prompt generated from feedback)
    options.Messages.Add(new ChatRequestUserMessage(prompt));

    // Configure request properties
    options.MaxTokens = 4096;
    options.Temperature = 0.7f;
    options.NucleusSamplingFactor = 0.95f;
    options.FrequencyPenalty = 0.0f;
    options.PresencePenalty = 0.0f;
    // options.StopSequences.Add("\n");
    options.DeploymentName = chatCompletionDeploymentName;

    options.ResponseFormat = ChatCompletionsResponseFormat.JsonObject;
    return options;
}

public static async Task<bool> GenerateUserStories(string csvFilePath, string jsonFilePath, string systemMessage, OpenAIClient openAIClient, string chatCompletionDeploymentName, string embeddingDeploymentName)
{
    using var reader = new StreamReader(csvFilePath);
    using var csv = new CsvHelper.CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);
    var records = csv.GetRecords<CSVFeedbackRecord>().ToList();

    // for testing will first do a sample of 10 records
    var count = 0;
    foreach (var record in records)
    {
        // Generate the user story from the feedback record
        record.UserStory = await CallOpenAI(openAIClient, GetOptions4UserStories(systemMessage, record.ToPrompt(), chatCompletionDeploymentName));

        // Generate the embedding for the user story
        record.Embedding = await GetEmbeddingAsync(record.UserStory, openAIClient, embeddingDeploymentName);
        Console.WriteLine($"User story & embedding generated for record with ID: {record.Id}");
        count++;
        if (count == 10)
        {
            Console.WriteLine("breaking after 10 records");
            break;
        }
    }

    // Serialize the records to a JSON file
    var json = JsonSerializer.Serialize(records, new JsonSerializerOptions { WriteIndented = true });
    await File.WriteAllTextAsync(jsonFilePath, json);
    return true;
}

public static async Task<List<ServiceCluster>> EnhanceClustersWithOpenAIAsync(OpenAIClient openAIClient, List<ServiceCluster> sortedClusters, string chatCompletionDeploymentName)
{
    int count = 0; // for testing, limit to 3 clusters
    var systemMessage = IOpenAIConstants.CreateClusterSystemMessage;
    List<ServiceCluster> resultClusters = new List<ServiceCluster>();
    foreach (var cluster in sortedClusters)
    {
        // Prepare the prompt by concatenating the feedback user stories for each cluster
        count++;
        string prompt = string.Empty;
        foreach (var feedback in cluster.FeedbackRecords)
        {
            prompt += $"- {feedback.UserStory}\n";
        }
        // Console.WriteLine($"Prompt: {prompt}");
        // Call OpenAI to generate the common element and summary
        var openAIResponse = await CallOpenAI(openAIClient, GetOptions4Clusters(prompt, systemMessage, chatCompletionDeploymentName));
        Console.WriteLine($"Called OpenAI  {cluster.ClusterId}");
                // Deserialize the JSON response from OpenAI
        try
            {
                var openAIResult = JsonSerializer.Deserialize<OpenAIResponse>(openAIResponse);

                if (openAIResult != null)
                {
                    // Update the cluster with OpenAI results
                    Console.WriteLine($"Common element: {openAIResult.CommonElement}\nSummary: {openAIResult.Summary}");
                    cluster.CommonElement = openAIResult.CommonElement;
                    cluster.Summary = openAIResult.Summary;
                    resultClusters.Add(cluster);
                }
                else
                {
                    Console.WriteLine("Failed to deserialize OpenAI response");
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON deserialization error: {ex.Message}");
            }
        // break after 3
        if (count == 3)
        {
            resultClusters = CleanClusterList(resultClusters);
            break;
        }
    }

    return resultClusters;
}

private static List<ServiceCluster> CleanClusterList(List<ServiceCluster> clusterList)
{
    foreach (var cluster in clusterList)
    {
        foreach (var feedback in cluster.FeedbackRecords)
        {
            // Set the Embedding field to null (or simply remove this line from the class definition if you don't need it)
            feedback.Embedding = [];
        }
    }

    return clusterList;
}

}
