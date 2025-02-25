namespace ProductLeaders.console.Utilities;

public class ClassificationUtility
{
    /// <summary>
    /// Extracts a classification for each user story by sending it to the LLM.
    /// This method is intended for research purposes, to help determine what classifications
    /// might be appropriate. You can later decide whether to use a predefined list or allow free-hand output.
    /// </summary>
    /// <param name="feedbackRecords">List of feedback records whose user stories need classification.</param>
    /// <param name="openAIClient">An instance of OpenAIClient for the API call.</param>
    /// <param name="chatCompletionDeploymentName">The deployment name for chat completions.</param>
    /// <returns>
    /// A list of tuples pairing each feedback record with the classification returned by the LLM.
    /// </returns>
    public static async Task<List<ClassifiedFeedback>> ExtractClassificationsForUserStories(
        List<FeedbackRecord> feedbackRecords,
        OpenAIClient openAIClient,
        string chatCompletionDeploymentName)
    {
        // System message instructing the LLM to classify a given user story.
        // You can adjust this text as needed.
        string systemMessage = IOpenAIConstants.UserStoryClassificationSystemMessage;

        var results = new List<ClassifiedFeedback>();

        // Process each user story (for now sequentially; you can parallelize if needed)
        foreach (var feedback in feedbackRecords)
        {
            // Build a prompt that asks for a classification.
            // For a predefined list, you could add something like: "Choose one of the following: [A, B, C, ...]"

            // Use the EnrichUtility helper to get ChatCompletionsOptions.
            // Here we assume that GetOptions4Clusters (or a similar method) is suitable for this purpose.
            ChatCompletionsOptions options = EnrichUtility.GetOptions4Classification(feedback.UserStory, systemMessage, chatCompletionDeploymentName);

            // Call the LLM to get the classification.
            string llmResponse = await EnrichUtility.CallOpenAI(openAIClient, options, "No classification returned.");

            //serlize the response
            if (string.IsNullOrWhiteSpace(llmResponse))
            {
                Console.WriteLine("No classification returned.");
                throw new ArgumentException("No classification returned.");
            }
            ClassificationLLMResponse ? classificationResponse = JsonSerializer.Deserialize<ClassificationLLMResponse>(llmResponse);
            if (classificationResponse == null)
            {
                Console.WriteLine("No classification returned.");
                throw new ArgumentException("No classification returned.");
            }
            Console.WriteLine($"Classification: {classificationResponse.Classification}");

            // Optionally, you could add additional normalization here (e.g., convert to title case)
            // to help later when merging similar classifications.
            results.Add(new ClassifiedFeedback { Feedback = feedback, Classification = classificationResponse.Classification });
        }

        return results;
    }
}

public class ClassificationLLMResponse
{
    [JsonPropertyName("classification")]
    public string Classification { get; set; } = string.Empty;

    [JsonPropertyName("reasoning")]
    public string Reasoning { get; set; } = string.Empty;
}

public class ClassifiedFeedback
{
    public FeedbackRecord Feedback { get; set; } = new FeedbackRecord();
    public string Classification { get; set; } = string.Empty;
}
