using System.Reflection.Metadata;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    const string SampleBasePath = "../FeedbackApi/DataSample";

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
}