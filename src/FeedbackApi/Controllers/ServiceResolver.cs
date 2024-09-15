public class ServiceResolver
{
    private readonly CosmosDbService _cosmosDbService;
    private readonly AksDbService _aksService;
    private readonly AdfDbService _adfService;

    public ServiceResolver(CosmosDbService cosmosDbService, AksDbService aksService, AdfDbService adfService)
    {
        _cosmosDbService = cosmosDbService;
        _aksService = aksService;
        _adfService = adfService;
    }

public VectorDbService Resolve(string serviceName)
{
    var normalizedServiceName = serviceName.ToLower() switch
    {
        "azure cosmos db" => "cosmosdb",
        "azure kubernetes service" => "aks",
        "azure data factory - data movement" => "adf",
        _ => null
    };

    if (normalizedServiceName == null)
    {
        throw new ArgumentException($"Invalid service name: {serviceName}");
    }

    return normalizedServiceName switch
    {
        "cosmosdb" => _cosmosDbService,
        "aks" => _aksService,
        "adf" => _adfService,
        _ => throw new ArgumentException($"Invalid service name: {serviceName}")
    };
}
}