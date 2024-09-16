using System.Reflection.Metadata;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    string localFolderPath = string.Empty;
    private readonly ServiceResolver _serviceResolver;

    public ServicesController(ServiceResolver serviceResolver)
    {
        _serviceResolver = serviceResolver;
        localFolderPath = Environment.GetEnvironmentVariable("DB_ROOT_FOLDER") ?? "DB_ROOT_FOLDER not found";
    }

    [HttpGet("GetServiceHighlights")]
    public IActionResult GetServiceHighlights()
    {
        // Load data from a local JSON file
        string localFolderPath = Environment.GetEnvironmentVariable("DB_ROOT_FOLDER") ?? "DB_ROOT_FOLDER not found";
        var jsonData = System.IO.File.ReadAllText($"{localFolderPath}/service-feedback.json");
        var serviceHighlights = JsonSerializer.Deserialize<List<ServiceHighlight>>(jsonData);

        // Process data, create summary statistics (e.g., total feedback, sentiment, etc.)
        // Return data as JSON response
        return Ok(serviceHighlights);
    }

    [HttpGet("GetServiceClusters/{serviceName}")]
    public IActionResult GetServiceClusters(string serviceName)
    {
        // Map service name to the corresponding JSON file
        var normalizedServiceName = serviceName.ToLower() switch
        {
            "azure cosmos db" => "cosmosdb",
            "azure kubernetes service" => "aks",
            "azure data factory - data movement" => "adf",
            _ => null
        };
        string jsonFileName;
        switch (normalizedServiceName)
        {
            case "cosmosdb":
                jsonFileName = "cosmosdb-clusters.json";
                break;
            case "azure kubernetes service (aks)":
            case "aks":
                jsonFileName = "aks-clusters.json";
                break;
            case "azure data factory":
            case "adf":
                jsonFileName = "adf-clusters.json";
                break;
            default:
                return NotFound($"Service {serviceName} not found.");
        }

        // Construct the full path to the JSON file
        var jsonFilePath = $"{localFolderPath}/{jsonFileName}";
        // Check if the file exists
        if (!System.IO.File.Exists(jsonFilePath))
        {
            return NotFound("The cluster file was not found.");
        }

        // Read the file and deserialize the data
        var jsonData = System.IO.File.ReadAllText(jsonFilePath);
        var serviceClusters = JsonSerializer.Deserialize<List<ServiceCluster>>(jsonData);

        return Ok(serviceClusters);
    }

    [HttpGet("GetSummaryByIssue/{serviceName}")]
    public async Task<IActionResult> GetSummaryByIssue(string serviceName, [FromQuery] string userQuery)
    {
        Console.WriteLine($"GetSummaryByIssue: {serviceName}, {userQuery}");
        // Maximum number of feedback items to consider
        int maxResults = IOpenAIConstants.MaxSimilarFeedbacks;
        float similarityThreshold = IOpenAIConstants.SimilarityThreshold;

       // Get the appropriate service based on serviceName
        var selectedService = _serviceResolver.Resolve(serviceName);
        if (selectedService == null)
        {
            return BadRequest($"Invalid service name: {serviceName}");
        }

        // Perform a similarity search on the feedback data
        var searchResults = await selectedService.SearchByDotProduct(userQuery, maxResults, similarityThreshold);

        // Extract the user stories from the feedback results
        var userStories = searchResults.Select(result => result.Item).ToList();

        if (userStories.Count == 0)
        {
            Console.WriteLine("No similar feedback found for the specified issue.");
            return NotFound("No similar feedback found for the specified issue.");
        }

    // Call OpenAI to summarize the user stories into a common summary
        var issueSummary = await selectedService.SummarizeFeedback(userStories, userQuery);

        // Add additional fields to the IssueSummary object
        issueSummary.SimilarIssues = userStories.Count;  // This matches 'similar_issues'
        issueSummary.DistinctCustomers = searchResults.Select(r => r.Item.CustomerName).Distinct().Count();  // This matches 'distinct_customers'
        issueSummary.FeedbackLinks = searchResults.Select(r => $"feedback_link_for_{r.Item.Id}").ToList();  // Matches 'feedback_links'

        // Return the structured summary in the response
        return Ok(issueSummary);
    }

    [HttpGet("GetCustomersByIssue/{serviceName}")]
    public async Task<IActionResult> GetCustomersByIssue(string serviceName, [FromQuery] string userQuery, int maxResults = IOpenAIConstants.MaxSimilarFeedbacks, float similarityThreshold = IOpenAIConstants.SimilarityThreshold)
    {
        // Get the appropriate service based on serviceName
        var selectedService = _serviceResolver.Resolve(serviceName);
        if (selectedService == null)
        {
            return BadRequest($"Invalid service name: {serviceName}");
        }

        // Search for matching records in the selected service
        var searchResults = await selectedService.SearchByDotProduct(userQuery, maxResults, similarityThreshold);

        if (searchResults == null || searchResults.Count == 0)
        {
            return NotFound("No matching feedback records found.");
        }
        Console.WriteLine($"GetCustomersByIssue: {serviceName}, {userQuery} has: {searchResults.Count} user stories matched");

        // Collect all user stories from the search results
        var userStories = searchResults.Select(result => result.Item.UserStory).ToList();

        // Call the helper method to generate a common user story
        string commonUserStory = await selectedService.GenerateCommonUserStory(userStories,userQuery);

        // Extract customer data from the search results and build the IssueData object
        var customers = searchResults.Select(result => new Customer
        {
            Name = result.Item.CustomerName,
            Tpid = result.Item.CustomerTpid,
            FeedbackTitle = result.Item.Title
        }).ToList();

        // Build the IssueData object
        var issueData = new IssueData
        {
            OriginalUserPrompt = userQuery, // Store the original query
            UserStory = commonUserStory,    // Use the generated common user story
            Customers = customers           // Add the list of customers
        };

        // Return the IssueData object as JSON
        return Ok(issueData);
    }

}
