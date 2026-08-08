using foodBridgeAPI.Data;
using foodBridgeAPI.Middleware;
using foodBridgeAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<GroqSettings>(builder.Configuration.GetSection("GroqSettings"));
builder.Services.AddHttpClient<IGroqService, GroqService>();

builder.Services.AddScoped<IDonacionService, DonacionService>();
builder.Services.AddScoped<ISolicitudService, SolicitudService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SupabaseConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.MapGet("/", () => Results.Redirect("/swagger"));

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
