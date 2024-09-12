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
    public IActionResult GetCustomersByIssue(string serviceName, [FromQuery] string userQuery)
    {
        // Assuming there's a JSON file containing customer data for issues
        string jsonFileName = $"{serviceName.ToLower()}-issue-customer-list.json";

        // Construct the full path to the JSON file
        var jsonFilePath = $"{SampleBasePath}/{jsonFileName}";
        Console.WriteLine($"path of customers: {jsonFilePath}");
        

        // Check if the file exists
        if (!System.IO.File.Exists(jsonFilePath))
        {
            return NotFound("The customer file was not found.");
        }
        // Read the file and output raw JSON for debugging
        var jsonData = System.IO.File.ReadAllText(jsonFilePath);
        
        // Deserialize into a Dictionary where the key is the issue name
    // Deserialize directly into IssueData
        var issueData = JsonSerializer.Deserialize<IssueData>(jsonData);

    // Update the OriginalUserPrompt with the user's query
        issueData.OriginalUserPrompt = userQuery;

        // Return the IssueData object as JSON
        return Ok(issueData);
    }
}