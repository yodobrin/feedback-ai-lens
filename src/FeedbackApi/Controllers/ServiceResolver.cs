public class ServiceResolver
{
    private readonly Dictionary<string, VectorDbService> _services;
    private readonly Dictionary<string, ServiceDescriptor> _serviceDescriptors;

    public ServiceResolver(Dictionary<string, VectorDbService> services, List<ServiceDescriptor> serviceDescriptors)
    {
        _services = services;
        _serviceDescriptors = serviceDescriptors.ToDictionary(
            s => s.MarketingName.Trim().ToLower(),
            s => s,
            StringComparer.OrdinalIgnoreCase
        );
    }

    public VectorDbService Resolve(string serviceName)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            throw new ArgumentException("Service name must not be empty.");
        }

        var key = serviceName.Trim().ToLower();

        if (_services.TryGetValue(key, out var service))
        {
            return service;
        }

        throw new ArgumentException($"Service '{serviceName}' not found.");
    }

    public string GetInternalId(string serviceName)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            throw new ArgumentException("Service name must not be empty.");
        }

        var key = serviceName.Trim().ToLower();

        if (_serviceDescriptors.TryGetValue(key, out var descriptor))
        {
            return descriptor.InternalId;
        }

        throw new ArgumentException($"Service '{serviceName}' not found in the service descriptors.");
    }

    public ServiceDescriptor GetServiceDescriptor(string serviceName)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            throw new ArgumentException("Service name must not be empty.");
        }

        var key = serviceName.Trim().ToLower();

        if (_serviceDescriptors.TryGetValue(key, out var descriptor))
        {
            return descriptor;
        }

        throw new ArgumentException($"Service '{serviceName}' not found in the service descriptors.");
    }
}
