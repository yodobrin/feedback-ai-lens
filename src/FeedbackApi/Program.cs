using DotNetEnv;
using Azure.AI.OpenAI;
using Azure;

var builder = WebApplication.CreateBuilder(args);

string configurationFile = Path.Combine(
    Directory.GetCurrentDirectory(),
    "../../configuration/.env"
);
Env.Load(configurationFile);

string localFolderPath =
    Environment.GetEnvironmentVariable("DB_ROOT_FOLDER") ?? "DB_ROOT_FOLDER not found";

var mappingFilePath = Path.Combine(localFolderPath, IOpenAIConstants.ServiceMappingFile);
var serviceMappingConfig = JsonSerializer.Deserialize<ServiceMappingConfig>(
    File.ReadAllText(mappingFilePath)
);

// Ensure service mappings are loaded
if (serviceMappingConfig?.Services == null || !serviceMappingConfig.Services.Any())
{
    throw new InvalidOperationException("Service mappings configuration is missing or empty.");
}

// Initialize OpenAIClient

var (oAiApiKey, oAiEndpoint, embeddingDeploymentName, chatCompletionDeploymentName) =
    GetOpenAIConfig();
AzureKeyCredential azureKeyCredential = new AzureKeyCredential(oAiApiKey);
var openAIClient = new OpenAIClient(new Uri(oAiEndpoint), azureKeyCredential);

// Create a dictionary to hold service instances
var serviceDictionary = new Dictionary<string, VectorDbService>(StringComparer.OrdinalIgnoreCase);

// Initialize services
foreach (var descriptor in serviceMappingConfig.Services)
{
    var service = new VectorDbService(embeddingDeploymentName, chatCompletionDeploymentName);

    // Use the internal ID and file patterns to initialize the service
    string jsonFileName = descriptor.FilePatterns.Vector;

    // Initialize the service
    service.InitializeAsync(jsonFileName, localFolderPath, openAIClient).Wait();

    // Use the marketing name as the key in the dictionary
    serviceDictionary[descriptor.MarketingName.Trim().ToLower()] = service;
}

// Register the list of ServiceDescriptors
builder.Services.AddSingleton(serviceMappingConfig.Services);

// Register the service dictionary and resolver
builder.Services.AddSingleton(serviceDictionary);
builder.Services.AddSingleton<ServiceResolver>(sp =>
{
    var services = sp.GetRequiredService<Dictionary<string, VectorDbService>>();
    return new ServiceResolver(services, serviceMappingConfig.Services);
});

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowLocalhost",
        builder => builder.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader()
    );
});

// Add the authorization service here
builder.Services.AddAuthorization();

builder.Services.AddSingleton<ServiceResolver>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable CORS
app.UseCors("AllowLocalhost");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static (
    string oAiApiKey,
    string oAiEndpoint,
    string embeddingDeploymentName,
    string chatCompletionDeploymentName
) GetOpenAIConfig()
{
    string oAiApiKey = Environment.GetEnvironmentVariable("AOAI_APIKEY") ?? "AOAI_APIKEY not found";
    string oAiEndpoint =
        Environment.GetEnvironmentVariable("AOAI_ENDPOINT") ?? "AOAI_ENDPOINT not found";
    string embeddingDeploymentName =
        Environment.GetEnvironmentVariable("EMBEDDING_DEPLOYMENTNAME")
        ?? "EMBEDDING_DEPLOYMENTNAME not found";
    string chatCompletionDeploymentName =
        Environment.GetEnvironmentVariable("CHATCOMPLETION_DEPLOYMENTNAME")
        ?? "CHATCOMPLETION_DEPLOYMENTNAME not found";
    return (oAiApiKey, oAiEndpoint, embeddingDeploymentName, chatCompletionDeploymentName);
}
