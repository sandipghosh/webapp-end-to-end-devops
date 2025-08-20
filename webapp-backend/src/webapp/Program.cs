using dataaccess;
using dataaccess.UnitOfWork;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using webapp.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure configuration sources
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // base
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true) // env-specific
    .AddEnvironmentVariables(); // Docker/env overrides

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();

// MySQL connection
/*
export WEBAPP_DB_HOST=localhost
export WEBAPP_DB_PORT=3306
export WEBAPP_DB_NAME=app_db
export WEBAPP_DB_USER=sandip
export WEBAPP_DB_PASSWORD=sandev@1984
*/

var config = builder.Configuration;

var dbHostName = Environment.GetEnvironmentVariable("WEBAPP_DB_HOST") ?? "localhost";
var dbHostPort = Environment.GetEnvironmentVariable("WEBAPP_DB_PORT") ?? "3306";
var dbName = Environment.GetEnvironmentVariable("WEBAPP_DB_NAME") ?? "app_db";
var dbUser = Environment.GetEnvironmentVariable("WEBAPP_DB_USER") ?? "sandip";
var dbPassword = Environment.GetEnvironmentVariable("WEBAPP_DB_PASSWORD") ?? "sandev@1984";
var clientURL = Environment.GetEnvironmentVariable("WEBAPP_CLIENT_URL") ?? "http://localhost:4200";

var logger = LoggerFactory.Create(b => b.AddConsole()).CreateLogger("Startup");

var connectionString = $"server={dbHostName};port={dbHostPort};database={dbName};user={dbUser};password={dbPassword};";
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
//    ?? "server=localhost;port=3306;database=demoapp;user=root;password=Password@123;";
logger.LogInformation("Using connection string: {conn}", connectionString);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(clientURL.Split(','))
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<TaskService>();

var app = builder.Build();

app.UseCors();
app.UseFastEndpoints();
app.UseSwaggerGen(); // Swagger UI at /swagger

app.Run();