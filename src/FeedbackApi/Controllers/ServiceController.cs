using System.Reflection.Metadata;


[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    const string SampleBasePath = "../FeedbackApi/DataSample";
    private readonly VectorDbService _cosmosDbService;
    private readonly VectorDbService _aksService;
    private readonly VectorDbService _adfService;

    public ServicesController(
        VectorDbService cosmosDbService,
        VectorDbService aksService,
        VectorDbService adfService)
    {
        _cosmosDbService = cosmosDbService;
        _aksService = aksService;
        _adfService = adfService;
    }

    [HttpGet("GetServiceHighlights")]
    public IActionResult GetServiceHighlights()
    {
        // Load data from a local JSON file 
        var jsonData = System.IO.File.ReadAllText($"{SampleBasePath}/service-feedback.json");
        var serviceHighlights = JsonSerializer.Deserialize<List<ServiceHighlight>>(jsonData);
        
        // Process data, create summary statistics (e.g., total feedback, sentiment, etc.)
        // Return data as JSON response
        return Ok(serviceHighlights);
    }

    [HttpGet("GetServiceClusters/{serviceName}")]
    public IActionResult GetServiceClusters(string serviceName)
    {
        // Map service name to the corresponding JSON file
        string jsonFileName;
        switch (serviceName.ToLower())
        {
            case "cosmosdb":
                jsonFileName = "cosmos-clusters.json";
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
        var jsonFilePath = $"{SampleBasePath}/{jsonFileName}";
        
        // Console.WriteLine(jsonFilePath);
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
    public IActionResult GetSummaryByIssue(string serviceName, [FromQuery] string userQuery)
    {
        string jsonFileName = $"{serviceName.ToLower()}-issue-summary.json";

        // Construct the full path to the JSON file, using the same approach as other APIs
        var jsonFilePath = $"{SampleBasePath}/{jsonFileName}";
        Console.WriteLine($"path of summary: {jsonFilePath}");

        // Check if the file exists
        if (!System.IO.File.Exists(jsonFilePath))
        {
            return NotFound("The issue summary file was not found.");
        }
        
        // Read the file and output raw JSON for debugging
        var jsonData = System.IO.File.ReadAllText(jsonFilePath);
            
        var issueSummary = JsonSerializer.Deserialize<IssueSummary>(jsonData);


        return Ok(issueSummary);
    }

    [HttpGet("GetCustomersByIssue/{serviceName}")]
    public async Task<IActionResult> GetCustomersByIssue(string serviceName, [FromQuery] string userQuery, int maxResults = 5, float similarityThreshold = 0.8f)
    {
        // Get the appropriate service based on serviceName
        var result = GetServiceByName(serviceName, out var selectedService);
        if (result != null)
        {
            return result; // Return BadRequest if the service name is invalid
        }

        // Search for matching records in the selected service
        var searchResults = await selectedService.SearchByDotProduct(userQuery, maxResults, similarityThreshold);

        if (searchResults == null || searchResults.Count == 0)
        {
            return NotFound("No matching feedback records found.");
        }

        // Collect all user stories from the search results
        var userStories = searchResults.Select(result => result.Item.UserStory).ToList();

        // Call the helper method to generate a common user story
        string commonUserStory = await selectedService.GenerateCommonUserStory(userStories);

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
    private IActionResult GetServiceByName(string serviceName, out VectorDbService selectedService)
    {
        selectedService = serviceName.ToLower() switch
        {
            "cosmosdb" => _cosmosDbService,
            "aks" => _aksService,
            "adf" => _adfService,
            _ => null
        };

        if (selectedService == null)
        {
            return BadRequest($"Invalid service name:{serviceName}!");
        }

        return null; // No error, service was found
    }
}