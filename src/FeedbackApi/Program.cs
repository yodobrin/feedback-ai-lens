using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

string configurationFile = Path.Combine(Directory.GetCurrentDirectory(),  "../../configuration/.env");
Env.Load(configurationFile);

string cosmosdbFileName = Environment.GetEnvironmentVariable("COSMOS_DB_FILE_NAME") ?? "cosmosdb-feedback-collection.json"; // default for ease of use 
string adfFileName = Environment.GetEnvironmentVariable("ADF_FILE_NAME") ?? "adf-feedback-collection.json";
string aksFileName = Environment.GetEnvironmentVariable("AKS_FILE_NAME") ?? "aks-feedback-collection.json";

Console.WriteLine($"Loading these collections to memory: CosmosDB: {cosmosdbFileName}, ADF: {adfFileName}, AKS: {aksFileName}");

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        builder => builder.WithOrigins("http://localhost:3000")
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});
// Add the authorization service here
builder.Services.AddAuthorization();
// Register a dedicated VectorDbService for each service
builder.Services.AddSingleton<VectorDbService>(sp =>
{
    var service = new VectorDbService();
    service.InitializeAsync(cosmosdbFileName).Wait();  // Initialize with CosmosDB file
    return service;
});

builder.Services.AddSingleton<VectorDbService>(sp =>
{
    var service = new VectorDbService();
    service.InitializeAsync(adfFileName).Wait();  // Initialize with ADF file
    return service;
});

builder.Services.AddSingleton<VectorDbService>(sp =>
{
    var service = new VectorDbService();
    service.InitializeAsync(aksFileName).Wait();  // Initialize with AKS file
    return service;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// // Resolve the service
// var vectorDbService = app.Services.GetRequiredService<VectorDbService>();
// // Perform the initialization
// await vectorDbService.InitializeAsync(cosmosdbFileName);

// Enable CORS
app.UseCors("AllowLocalhost");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();