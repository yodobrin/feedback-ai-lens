
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
        await _dbService.InitializeAsync(jsonFileName);
        return Ok("Reloaded DB");
    }

}
