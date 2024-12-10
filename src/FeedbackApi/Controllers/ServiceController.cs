using System.Reflection.Metadata;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    readonly string _localFolderPath = string.Empty;
    private readonly ServiceResolver _serviceResolver;

    public ServicesController(ServiceResolver serviceResolver)
    {
        _serviceResolver = serviceResolver;
        _localFolderPath =
            Environment.GetEnvironmentVariable("DB_ROOT_FOLDER") ?? "DB_ROOT_FOLDER not found";
    }

    [HttpGet("GetInternalId")]
    public IActionResult GetInternalId([FromQuery] string serviceName)
    {
        try
        {
            var internalId = _serviceResolver.GetInternalId(serviceName);
            return Ok(new { InternalId = internalId });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetServiceHighlights")]
    public IActionResult GetServiceHighlights()
    {
        // Load data from a local JSON file
        var serviceHighlightsFilePath = Path.Combine(
            _localFolderPath,
            IOpenAIConstants.CombinedSummaryFile
        );
        var jsonData = System.IO.File.ReadAllText($"{serviceHighlightsFilePath}"); // new file name
        var serviceHighlights = JsonSerializer.Deserialize<List<ServiceHighlight>>(jsonData);
        // Return data as JSON response
        return Ok(serviceHighlights);
    }

    [HttpGet("GetServiceClusters/{serviceName}")]
    public IActionResult GetServiceClusters(string serviceName)
    {
        Console.WriteLine($"GetServiceClusters: {serviceName}");
        try
        {
            // Get the service descriptor
            var serviceDescriptor = _serviceResolver.GetServiceDescriptor(serviceName);
            string clustersFileName = Path.Combine(
                _localFolderPath,
                serviceDescriptor.FilePatterns.Clusters
            );
            var jsonData = System.IO.File.ReadAllText(clustersFileName);
            var serviceClusters = JsonSerializer.Deserialize<List<ServiceCluster>>(jsonData);
            return Ok(serviceClusters);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetSummaryByIssue/{serviceName}")]
    public async Task<IActionResult> GetSummaryByIssue(
        string serviceName,
        [FromQuery] string userQuery
    )
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
        long start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        var searchResults = await selectedService.SearchByDotProduct(
            userQuery,
            maxResults,
            similarityThreshold
        );
        Console.WriteLine(
            $"Vector search took {DateTimeOffset.Now.ToUnixTimeMilliseconds() - start} ms"
        );
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
        issueSummary.SimilarIssues = userStories.Count; // This matches 'similar_issues'
        issueSummary.DistinctCustomers = searchResults
            .Select(r => r.Item.CustomerName)
            .Distinct()
            .Count(); // This matches 'distinct_customers'
        issueSummary.FeedbackLinks = searchResults
            .Select(r => $"feedback_link_for_{r.Item.Id}")
            .ToList(); // Matches 'feedback_links'

        // Return the structured summary in the response
        return Ok(issueSummary);
    }

    [HttpGet("GetCustomersByIssue/{serviceName}")]
    public async Task<IActionResult> GetCustomersByIssue(
        string serviceName,
        [FromQuery] string userQuery,
        int maxResults = IOpenAIConstants.MaxSimilarFeedbacks,
        float similarityThreshold = IOpenAIConstants.SimilarityThreshold
    )
    {
        // Get the appropriate service based on serviceName
        var selectedService = _serviceResolver.Resolve(serviceName);
        if (selectedService == null)
        {
            return BadRequest($"Invalid service name: {serviceName}");
        }

        // Search for matching records in the selected service
        long start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        var searchResults = await selectedService.SearchByDotProduct(
            userQuery,
            maxResults,
            similarityThreshold
        );
        Console.WriteLine(
            $"Vector search took {DateTimeOffset.Now.ToUnixTimeMilliseconds() - start} ms"
        );

        if (searchResults == null || searchResults.Count == 0)
        {
            return NotFound("No matching feedback records found.");
        }
        Console.WriteLine(
            $"GetCustomersByIssue: {serviceName}, {userQuery} has: {searchResults.Count} user stories matched"
        );

        // we need the entire object sent to generate the common user story
        var feedbackItems = searchResults.Select(result => result.Item).ToList();
        // Call the helper method to generate a common user story

        IssueData issueData = await selectedService.GenerateCommonUserStory(
            feedbackItems,
            userQuery
        );

        // Now, attach the corresponding feedback records to each customer
        foreach (var customer in issueData.Customers)
        {
            // Find the feedback records matching this customer (using customer name o)
            customer.FeedbackRecords = feedbackItems
                .Where(fb => fb.CustomerName == customer.Name)
                .ToList();
        }

        // Return the IssueData object as JSON
        return Ok(issueData);
    }
}
