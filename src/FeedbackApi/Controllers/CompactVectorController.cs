
[ApiController]
[Route("[controller]")]
public class CompactVectorController : ControllerBase
{
    private readonly VectorDbService _dbService;
    public CompactVectorController(VectorDbService dbService)
    {
        _dbService = dbService;
    }
    // Implement your API logic here

    [HttpGet("reload")]
    public async Task<IActionResult> Get(string jsonFileName)
    {
        string localFolderPath = Environment.GetEnvironmentVariable("DB_ROOT_FOLDER") ?? "DB_ROOT_FOLDER not found";
        // TODO decide if to remove the controller, it is not used.
        await _dbService.InitializeAsync(jsonFileName,localFolderPath,null);
        return Ok("Reloaded DB");
    }

}
