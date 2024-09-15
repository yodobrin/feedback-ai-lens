using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

string configurationFile = Path.Combine(Directory.GetCurrentDirectory(),  "../../configuration/.env");
Env.Load(configurationFile);

string cosmosdbFileName = Environment.GetEnvironmentVariable("COSMOS_DB_FILE_NAME") ?? "cosmosdb.json"; // default for ease of use 
string adfFileName = Environment.GetEnvironmentVariable("ADF_FILE_NAME") ?? "adf.json";
string aksFileName = Environment.GetEnvironmentVariable("AKS_FILE_NAME") ?? "aks.json";

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
builder.Services.AddSingleton<CosmosDbService>(sp =>
{
    var service = new CosmosDbService();
    service.InitializeAsync(cosmosdbFileName).Wait();
    return service;
});

builder.Services.AddSingleton<AksDbService>(sp =>
{
    var service = new AksDbService();
    service.InitializeAsync(aksFileName).Wait();
    return service;
});

builder.Services.AddSingleton<AdfDbService>(sp =>
{
    var service = new AdfDbService();
    service.InitializeAsync(adfFileName).Wait();
    return service;
});
// Register the ServiceResolver singleton
builder.Services.AddSingleton<ServiceResolver>(sp =>
{
    var cosmosDbService = sp.GetRequiredService<CosmosDbService>();
    var aksService = sp.GetRequiredService<AksDbService>();
    var adfService = sp.GetRequiredService<AdfDbService>();

    return new ServiceResolver(cosmosDbService, aksService, adfService);
});


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