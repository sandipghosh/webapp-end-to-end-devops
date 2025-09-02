using dataaccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using webapp;

namespace webapp.test.integration.testing;

public class CustomWebApplicationFactory: WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;

    public CustomWebApplicationFactory()
    {
        // 👇 One shared connection for all DbContexts
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing ApplicationDbContext
            var descriptors = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>) ||
                    d.ServiceType == typeof(ApplicationDbContext))
                .ToList();

            foreach (var descriptor in descriptors)
                services.Remove(descriptor);

            // 1. Find all DbContext-related services
            var dbDescriptors = services
                .Where(d => d.ServiceType.FullName!.Contains("DbContext"))
                .ToList();

            foreach (var d in dbDescriptors)
                services.Remove(d);

            // 2. Find and remove provider services (MySQL)
            var providerDescriptors = services
                .Where(d => d.ImplementationType?.Namespace?.Contains("Pomelo.EntityFrameworkCore.MySql") ?? false)
                .ToList();

            foreach (var d in providerDescriptors)
                services.Remove(d);

            // Use the shared SQLite connection
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(_connection));

            // Ensure DB schema exists
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
