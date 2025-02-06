
sealed class Program
{
    public static async Task Main(string[] args)
    {
        // Check if the user is asking for help or provided no arguments.
        if (args.Length == 0 ||
            args[0].Equals("-h", StringComparison.OrdinalIgnoreCase) ||
            args[0].Equals("--help", StringComparison.OrdinalIgnoreCase) ||
            args[0].Equals("help", StringComparison.OrdinalIgnoreCase))
        {
            ShowUsage();
            return;
        }

        try
        {
            // First argument: run config file
            string runConfigFile = args[0];
            // Second argument: environment file
            string envFile = args.Length > 1 ? args[1] : throw new ArgumentException("Please provide the path to the environment file.");
            Console.WriteLine($"Loading RunConfig from: {runConfigFile}, using system environment file: {envFile}");
            Env.Load(envFile);

            // Load environment variables
            string oAiApiKey = Environment.GetEnvironmentVariable("AOAI_APIKEY") ?? "AOAI_APIKEY not found";
            string oAiEndpoint = Environment.GetEnvironmentVariable("AOAI_ENDPOINT") ?? "AOAI_ENDPOINT not found";
            string chatCompletionDeploymentName = Environment.GetEnvironmentVariable("CHATCOMPLETION_DEPLOYMENTNAME") ?? "CHATCOMPLETION_DEPLOYMENTNAME not found";
            string embeddingDeploymentName = Environment.GetEnvironmentVariable("EMBEDDING_DEPLOYMENTNAME") ?? "EMBEDDING_DEPLOYMENTNAME not found";

            // Create the OpenAI client
            AzureKeyCredential azureKeyCredential = new AzureKeyCredential(oAiApiKey);
            OpenAIClient openAIClient = new OpenAIClient(new Uri(oAiEndpoint), azureKeyCredential);
            Console.WriteLine($"OpenAI Client created: {oAiEndpoint} with deployments: {chatCompletionDeploymentName} and {embeddingDeploymentName}");

            // Parse the run configuration
            RunConfig runConfig = InputParser.Parse(runConfigFile);
            Console.WriteLine("RunConfig loaded successfully:");
            Console.WriteLine($"  Input File: {runConfig.InputFile}");
            Console.WriteLine($"  Output Directory: {runConfig.OutputDirectory}");
            Console.WriteLine($"  Data Folder: {runConfig.DataFolder}");
            Console.WriteLine($"  Operation: {runConfig.Operation}");

            // // Extract and validate the service mapping file path
            // string serviceMappingFileName = GetParameterValue(runConfig.Parameters, "service_mapping_file") ?? string.Empty;
            // if (string.IsNullOrWhiteSpace(serviceMappingFileName))
            // {
            //     Console.WriteLine("Service mapping file not specified in parameters. Exiting.");
            //     return;
            // }
            // string serviceMappingPath = Path.IsPathRooted(serviceMappingFileName)
            //     ? serviceMappingFileName
            //     : Path.Combine(runConfig.OutputDirectory, serviceMappingFileName);

            // if (!File.Exists(serviceMappingPath))
            // {
            //     Console.WriteLine($"Service mapping configuration file not found: {serviceMappingPath}");
            //     return;
            // }
            // Console.WriteLine($"Loading ServiceMappingConfig from: {serviceMappingPath}");
            // var serviceMappingConfig = ServiceMappingConfigLoader.Load(serviceMappingPath);
            // Console.WriteLine($"ServiceMappingConfig loaded successfully for service: {runConfig.Service}.");

            // Delegate to the correct operation
            switch (runConfig.Operation.ToLower())
            {
                case "summary":
                    ProcessSummaryOperation(runConfig);
                    break;

                case "json":
                    await ProcessJsonOperation(runConfig, openAIClient, chatCompletionDeploymentName, embeddingDeploymentName);
                    break;

                case "cluster":
                    await ProcessClusterOperation(runConfig, openAIClient, chatCompletionDeploymentName);
                    break;

                case "search":
                    await ProcessSearchOperation(runConfig, openAIClient, embeddingDeploymentName, chatCompletionDeploymentName);
                    break;

                default:
                    Console.WriteLine($"Unsupported operation: {runConfig.Operation}");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Displays help and usage instructions for the CLI.
    /// </summary>
    private static void ShowUsage()
    {
        Console.WriteLine("Usage: MyCliApp <RunConfigFile> <EnvFile>");
        Console.WriteLine();
        Console.WriteLine("RunConfigFile is a JSON file that specifies the run configuration. An example structure is:");
        Console.WriteLine(@"
{
  ""service"": ""adf"",
  ""input_file"": ""../../sample-data/adf.csv"",
  ""data_folder"": ""../../sample-data/"",
  ""output_directory"": ""results/"",
  ""operation"": ""search"",
  ""parameters"": {
    ""service_mapping_file"": ""service_mapping.json"",
    ""embedding_dimension"": 1536,
    ""similarity_threshold"": 0.75,
    // For the search operation only:
    ""search_query"": ""need for improved security measures and streamlined authentication processes across various role"",
    ""query_type"": ""find_customers""
  }
}
        ");
        Console.WriteLine();
        Console.WriteLine("EnvFile is a file containing the required environment variables:");
        Console.WriteLine("  AOAI_APIKEY");
        Console.WriteLine("  AOAI_ENDPOINT");
        Console.WriteLine("  CHATCOMPLETION_DEPLOYMENTNAME");
        Console.WriteLine("  EMBEDDING_DEPLOYMENTNAME");
        Console.WriteLine();
        Console.WriteLine("Operations:");
        Console.WriteLine("  summary   - Generate a summary from the input file.");
        Console.WriteLine("  json      - Generate user stories from feedback using OpenAI.");
        Console.WriteLine("  cluster   - Cluster feedback records based on their embeddings.");
        Console.WriteLine("  search    - Perform a vector search using embeddings. Requires additional parameters:");
        Console.WriteLine("              search_query: The query string for searching.");
        Console.WriteLine("              query_type: Either 'find_customers' or 'find_use_cases'.");
        Console.WriteLine();
        Console.WriteLine("Example:");
        Console.WriteLine("  dotnet run config.json env.txt");
    }

    /// <summary>
    /// Safely extracts a parameter value as a string from the parameters dictionary.
    /// Handles both JsonElement and plain string types.
    /// </summary>
    private static string? GetParameterValue(Dictionary<string, object> parameters, string key)
    {
        if (parameters.TryGetValue(key, out var value))
        {
            if (value is JsonElement jsonElement)
            {
                return jsonElement.GetString();
            }
            return value?.ToString();
        }
        return null;
    }

    /// <summary>
    /// Generates a unique output file name using the service name, an abbreviated query type, and a new GUID.
    /// </summary>
    private static string GenerateUniqueOutputFileName(string service, string queryType)
    {
        string abbreviation = queryType switch
        {
            "find_customers" => "fc",
            "find_use_cases" => "fu",
            _ => queryType.Length >= 2 ? queryType.Substring(0, 2) : queryType
        };

        return $"{service}_{abbreviation}_{Guid.NewGuid()}.json";
    }

    // ----------------- Operation Methods -----------------

    private static void ProcessSummaryOperation(RunConfig runConfig)
    {
        Console.WriteLine("Executing summary generation...");
        var summary = SummaryGenerator.GenerateSummary(runConfig.InputFile);
        string outputFile = $"{runConfig.Service}_{runConfig.Operation}.json";
        SummaryGenerator.SaveSummary(Path.Combine(runConfig.OutputDirectory, outputFile), summary);
        Console.WriteLine($"Summary saved to: {Path.Combine(runConfig.OutputDirectory, outputFile)}");
    }

    private static async Task ProcessJsonOperation(RunConfig runConfig, OpenAIClient openAIClient, string chatCompletionDeploymentName, string embeddingDeploymentName)
    {
        Console.WriteLine($"Executing user story generation with system message: {IOpenAIConstants.Feedback2UserStory}");
        string outputFile = $"{runConfig.Service}.json";
        bool success = await EnrichUtility.GenerateUserStories(
            runConfig.InputFile,
            Path.Combine(runConfig.OutputDirectory, outputFile),
            IOpenAIConstants.Feedback2UserStory,
            openAIClient,
            chatCompletionDeploymentName,
            embeddingDeploymentName
        );
    }

    private static async Task ProcessClusterOperation(RunConfig runConfig, OpenAIClient openAIClient, string chatCompletionDeploymentName)
    {
        string serviceDataFile = $"{runConfig.Service}.json";
        string serviceDataPath = Path.Combine(runConfig.OutputDirectory, serviceDataFile);
        string jsonString = File.ReadAllText(serviceDataPath);
        var feedbackRecords = JsonSerializer.Deserialize<List<FeedbackRecord>>(jsonString)
                              ?? throw new InvalidOperationException("No feedback records found");

        var rawClusters = ClusterUtility.CreateClusters(feedbackRecords, 50);
        var serviceClusters = ClusterUtility.GenerateClusters(rawClusters);
        var sortedClusters = serviceClusters
            .OrderByDescending(cluster => cluster.DistinctCustomers)
            .ThenByDescending(cluster => cluster.SimilarFeedbacks)
            .ToList();

        var clusters = await EnrichUtility.EnhanceClustersWithOpenAIAsync(openAIClient, sortedClusters, chatCompletionDeploymentName);
        string outputFile = $"{runConfig.Service}_clusters_full.json";
        var clusterJson = JsonSerializer.Serialize(clusters, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(runConfig.OutputDirectory, outputFile), clusterJson);
        Console.WriteLine($"Clusters saved to: {Path.Combine(runConfig.OutputDirectory, outputFile)}");
    }

    private static async Task ProcessSearchOperation(RunConfig runConfig, OpenAIClient openAIClient, string embeddingDeploymentName, string chatCompletionDeploymentName)
    {
        // Extract and validate search parameters
        string? queryType = GetParameterValue(runConfig.Parameters, "query_type");
        string? searchQuery = GetParameterValue(runConfig.Parameters, "search_query");

        if (string.IsNullOrWhiteSpace(searchQuery) ||
            string.IsNullOrWhiteSpace(queryType) ||
            (queryType != "find_customers" && queryType != "find_use_cases"))
        {
            Console.WriteLine($"Search parameters invalid. query_type: {queryType}, search_query: {searchQuery} must be specified (query_type must be either 'find_customers' or 'find_use_cases'). Exiting.");
            return;
        }

        // Build the full path to the service data file
        string serviceJsonFile = $"{runConfig.Service}.json";
        string serviceJsonPath = Path.Combine(runConfig.DataFolder, serviceJsonFile);

        if (!File.Exists(serviceJsonPath))
        {
            Console.WriteLine($"Service data file not found at: {serviceJsonPath}. Exiting.");
            return;
        }

        string serviceJsonString = File.ReadAllText(serviceJsonPath);
        if (string.IsNullOrWhiteSpace(serviceJsonString))
        {
            Console.WriteLine("Service data file is empty. Exiting.");
            return;
        }

        // Initialize the vector database service
        var vectorDbService = new VectorDbService(embeddingDeploymentName, chatCompletionDeploymentName);
        await vectorDbService.InitializeAsync(serviceJsonFile, runConfig.DataFolder, openAIClient);

        int maxResults = IOpenAIConstants.MaxSimilarFeedbacks;
        float similarityThreshold = IOpenAIConstants.SimilarityThreshold;

        if (queryType == "find_use_cases")
        {
            Console.WriteLine("Executing search: find_use_cases");
            var searchResults = await vectorDbService.SearchByDotProduct(searchQuery, maxResults, similarityThreshold);
            var searchFeedbackRecords = searchResults.Select(result => result.Item).ToList();

            if (searchFeedbackRecords.Count == 0)
            {
                Console.WriteLine("No similar feedback found for the specified issue. Exiting.");
                return;
            }

            var issueSummary = await vectorDbService.SummarizeFeedback(searchFeedbackRecords, searchQuery);
            issueSummary.SimilarIssues = searchFeedbackRecords.Count;
            issueSummary.DistinctCustomers = searchFeedbackRecords.Select(r => r.CustomerName).Distinct().Count();
            issueSummary.FeedbackLinks = searchFeedbackRecords.Select(r => $"feedback_link_for_{r.Id}").ToList();

            string outputFile = GenerateUniqueOutputFileName(runConfig.Service, queryType);
            var summaryJson = JsonSerializer.Serialize(issueSummary, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Path.Combine(runConfig.OutputDirectory, outputFile), summaryJson);
            Console.WriteLine($"Issue summary saved to: {Path.Combine(runConfig.OutputDirectory, outputFile)}");
        }
        else if (queryType == "find_customers")
        {
            Console.WriteLine("Executing search: find_customers");
            var searchResults = await vectorDbService.SearchByDotProduct(searchQuery, maxResults, similarityThreshold);
            var feedbackItems = searchResults.Select(result => result.Item).ToList();

            if (feedbackItems.Count == 0)
            {
                Console.WriteLine("No similar feedback found for the specified issue. Exiting.");
                return;
            }

            var issueData = await vectorDbService.GenerateCommonUserStory(feedbackItems, searchQuery);
            foreach (var customer in issueData.Customers)
            {
                customer.FeedbackRecords = feedbackItems.Where(fb => fb.CustomerName == customer.Name).ToList();
            }

            string outputFile = GenerateUniqueOutputFileName(runConfig.Service, queryType);
            var issueDataJson = JsonSerializer.Serialize(issueData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Path.Combine(runConfig.OutputDirectory, outputFile), issueDataJson);
            Console.WriteLine($"Issue data saved to: {Path.Combine(runConfig.OutputDirectory, outputFile)}");
        }
    }
}
