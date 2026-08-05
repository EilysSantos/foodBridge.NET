using foodBridgeAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SupabaseConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(() =>
{
    var url = app.Urls.FirstOrDefault() ?? "http://localhost:5287";
    Console.WriteLine();
    Console.WriteLine("========================================");
    Console.WriteLine(" Servidor FoodBridge API corriendo");
    Console.WriteLine($" API:     {url}");
    Console.WriteLine($" Swagger: {url}/swagger");
    Console.WriteLine("========================================");
    Console.WriteLine();
});

app.Run();
