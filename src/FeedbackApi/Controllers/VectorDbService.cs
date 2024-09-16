// using System.IO;
using Azure.AI.OpenAI;
using Azure;
// using Azure.Core;

public class VectorDbService
{
    public VectorCollection? VectorCollection { get; private set; }
    private OpenAIClient? _openAIClient;
    private string? _embeddingDeploymentName;
    private string? _chatCompletionDeploymentName;

    public VectorDbService()
    {
        Console.WriteLine("VectorDbService constructor called");
    }
    public async Task<SearchResult> SearchByDotProduct(string query)
    {
        // check the vector collection is not null throw exception
        if (VectorCollection == null)
        {
            throw new ArgumentException("VectorCollection is null");
        }
        var queryVector = await GetEmbeddings(query);
        return VectorCollection.FindByDotProduct(queryVector, item => item.GetVector());
    }
    public async Task<SearchResult> SearchByCosineSimilarity(string query)
    {
        // check the vector collection is not null throw exception
        if (VectorCollection == null)
        {
            throw new ArgumentException("VectorCollection is null");
        }
        var queryVector = await GetEmbeddings(query);
        return VectorCollection.FindByCosineSimilarity(queryVector, item => item.GetVector());
    }
    public async Task<SearchResult> SearchByEuclideanDistance(string query)
    {
        // check the vector collection is not null throw exception
        if (VectorCollection == null)
        {
            throw new ArgumentException("VectorCollection is null");
        }
        var queryVector = await GetEmbeddings(query);
        return VectorCollection.FindByEuclideanDistance(queryVector, item => item.GetVector());
    }

    // Helper method to call OpenAI
    public async Task<string> CallOpenAI(string prompt, string systemMessage, bool useJson = false)
    {
        // Check for null onopenAIClient
        if (_openAIClient == null)
        {
            throw new ArgumentException("OpenAI Client is null");
        }
        ChatCompletionsOptions options = new ChatCompletionsOptions
        {
            MaxTokens = 4096,
            Temperature = 0.7f,
            NucleusSamplingFactor = 0.95f,
            FrequencyPenalty = 0.0f,
            PresencePenalty = 0.0f
        };

        // Add system message
        options.Messages.Add(new ChatRequestSystemMessage(systemMessage));

        // Add user message (the prompt generated from feedback)
        options.Messages.Add(new ChatRequestUserMessage(prompt));

        // Stop sequences to end chat completions
        // options.StopSequences.Add("\n");
        if (useJson)
        {
            options.ResponseFormat = ChatCompletionsResponseFormat.JsonObject;
        }

        // Specify the deployment model
        options.DeploymentName = _chatCompletionDeploymentName;
        // Make the API request to get the chat completions
        Response<ChatCompletions> response = await _openAIClient.GetChatCompletionsAsync(options);

        // Extract and return the first response from the choices
        ChatCompletions completions = response.Value;
        if (completions.Choices.Count > 0)
        {
            return completions.Choices[0].Message.Content;
        }
        else
        {
            return "No response generated.";
        }
    }
    private async Task<float[]> GetEmbeddings(string query)
    {
        // null check for embeddingDeploymentName & openAIClient throw exception
        if (_embeddingDeploymentName == null || _openAIClient == null)
        {
            throw new ArgumentException("OpenAI Client or Embedding Deployment Name is null");
        }

        EmbeddingsOptions embeddingsOptions = new EmbeddingsOptions(_embeddingDeploymentName, new List<string> { query });
        var embeddingsResponse = await _openAIClient.GetEmbeddingsAsync(embeddingsOptions);
        return embeddingsResponse.Value.Data[0].Embedding.ToArray();
    }
    // Add GenerateCommonUserStory to utilize CallOpenAI
    public async Task<string> GenerateCommonUserStory(List<string> userStories, string originalQuery)
    {
        if (_openAIClient == null || string.IsNullOrEmpty(_chatCompletionDeploymentName))
        {
            throw new ArgumentException("OpenAI Client or model deployment name is not initialized.");
        }

        // Create the prompt based on the list of user stories
        string prompt = "Here are several user stories from different customers:\n\n";
        foreach (var story in userStories)
        {
            prompt += $"- {story}\n";
        }

        // Use string interpolation to embed the user query in the system message from the interface
        string systemMessage = string.Format(IOpenAIConstants.CommonUserStorySystemMessage, originalQuery);
        // Call OpenAI to generate the common user story

        return await CallOpenAI(prompt, systemMessage);
    }

    public async Task<IssueSummary> SummarizeFeedback(List<FeedbackRecord> feedbackItems, string originalQuery)
    {
        if (_openAIClient == null || string.IsNullOrEmpty(_chatCompletionDeploymentName))
        {
            throw new ArgumentException("OpenAI Client or model deployment name is not initialized.");
        }

        // Generate the prompt based on the feedback items
        string prompt = "Here are several feedback items from different customers,:\n\n";
        foreach (var feedback in feedbackItems)
        {
            prompt += $"- {feedback.Title}: {feedback.Description}\n";
        }
        prompt += $"here is the user query:{originalQuery}. Make sure to respond in json format";

        // Call OpenAI to generate the common element and summary
        var openAIResponse = await CallOpenAI(prompt, IOpenAIConstants.FeedbackSummarizationSystemMessage, true);
        Console.WriteLine($"OpenAI response: {openAIResponse}");
        // Deserialize the OpenAI response into IssueSummary structure
        try
        {
            var openAIResult = JsonSerializer.Deserialize<IssueSummary>(openAIResponse);
            if (openAIResult != null)
            {
                return openAIResult;
            }
            else
            {
                throw new ArgumentException("OpenAI response deserialization returned null.");
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error during JSON deserialization: {ex.Message}");
            throw;
        }
    }
    private async Task LoadDataFromLocalFolder(string jsonFileName)
    {
        // Get the path to the local folder (you can customize this)
        string localFolderPath = Environment.GetEnvironmentVariable("DB_ROOT_FOLDER") ?? "DB_ROOT_FOLDER not found";

        // Check if the environment variable is set correctly
        if (localFolderPath == "DB_ROOT_FOLDER not found" || string.IsNullOrEmpty(jsonFileName))
        {
            Console.WriteLine("One or more environment variables are not set. Please set DB_ROOT_FOLDER and ensure jsonFileName is not empty.");
            return;
        }

        // Construct the full file path
        string fullFilePath = Path.Combine(localFolderPath, jsonFileName);

        // Check if the file exists
        if (File.Exists(fullFilePath))
        {
            try
            {
                // Read the file asynchronously
                using (FileStream fileStream = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read))
                {
                    // Pass the stream to your existing VectorCollection logic
                    VectorCollection = await VectorCollection.CreateFromMemoryAsync(fileStream);
                }

                Console.WriteLine("File loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while reading the file: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"File does not exist: {fullFilePath}");
        }
    }

    public async Task InitializeAsync(string jsonFileName)
    {
        Console.WriteLine("Initializing VectorDbService & OpenAI Client");

        await LoadDataFromLocalFolder(jsonFileName);
        string oAiApiKey = Environment.GetEnvironmentVariable("AOAI_APIKEY") ?? "AOAI_APIKEY not found";
        string oAiEndpoint = Environment.GetEnvironmentVariable("AOAI_ENDPOINT") ?? "AOAI_ENDPOINT not found";
        _embeddingDeploymentName = Environment.GetEnvironmentVariable("EMBEDDING_DEPLOYMENTNAME") ?? "EMBEDDING_DEPLOYMENTNAME not found";
        _chatCompletionDeploymentName = Environment.GetEnvironmentVariable("CHATCOMPLETION_DEPLOYMENTNAME") ?? "CHATCOMPLETION_DEPLOYMENTNAME not found";
        AzureKeyCredential azureKeyCredential = new AzureKeyCredential(oAiApiKey);
        _openAIClient = new OpenAIClient(new Uri(oAiEndpoint), azureKeyCredential);
        Console.WriteLine("... Initialized VectorDbService & OpenAI Client !");
    }
    // Enhanced search method for dot product
    public async Task<List<SearchResult>> SearchByDotProduct(string query, int maxResults, float similarityThreshold)
    {
        if (VectorCollection == null)
        {
            throw new ArgumentException("VectorCollection is null");
        }

        var queryVector = await GetEmbeddings(query);
        return VectorCollection.FindByDotProduct(queryVector, item => item.GetVector(), maxResults, similarityThreshold);
    }

    // Enhanced search method for cosine similarity
    public async Task<List<SearchResult>> SearchByCosineSimilarity(string query, int maxResults, float similarityThreshold)
    {
        if (VectorCollection == null)
        {
            throw new ArgumentException("VectorCollection is null");
        }

        var queryVector = await GetEmbeddings(query);
        return VectorCollection.FindByCosineSimilarity(queryVector, item => item.GetVector(), maxResults, similarityThreshold);
    }

    // Enhanced search method for Euclidean distance
    public async Task<List<SearchResult>> SearchByEuclideanDistance(string query, int maxResults, float similarityThreshold)
    {
        if (VectorCollection == null)
        {
            throw new ArgumentException("VectorCollection is null");
        }

        var queryVector = await GetEmbeddings(query);
        return VectorCollection.FindByEuclideanDistance(queryVector, item => item.GetVector(), maxResults, similarityThreshold);
    }
}
// Define derived classes for each service type
public class CosmosDbService : VectorDbService { }
public class AksDbService : VectorDbService { }
public class AdfDbService : VectorDbService { }
