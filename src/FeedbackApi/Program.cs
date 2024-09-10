var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        builder => builder.WithOrigins("http://localhost:3000")
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

builder.Services.AddControllers();

var app = builder.Build();

// Enable CORS
app.UseCors("AllowLocalhost");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();