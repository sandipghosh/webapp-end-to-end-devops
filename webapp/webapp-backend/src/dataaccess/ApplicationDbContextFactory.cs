using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace dataaccess;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        var isTestConnectionActivated = bool.TryParse(Environment.GetEnvironmentVariable("TEST_USE_INMEMORY"), out var parsed) ? parsed : false;
        var dbHostName = Environment.GetEnvironmentVariable("WEBAPP_DB_HOST") ?? "localhost";
        var dbHostPort = Environment.GetEnvironmentVariable("WEBAPP_DB_PORT") ?? "3306";
        var dbName = Environment.GetEnvironmentVariable("WEBAPP_DB_NAME") ?? "app_db";
        var dbUser = Environment.GetEnvironmentVariable("WEBAPP_DB_USER") ?? "sandip";
        var dbPassword = Environment.GetEnvironmentVariable("WEBAPP_DB_PASSWORD") ?? "sandev@1984";
        var clientURL = Environment.GetEnvironmentVariable("WEBAPP_CLIENT_URL") ?? "http://localhost:4200";

        var connectionString = $"server={dbHostName};port={dbHostPort};database={dbName};user={dbUser};password={dbPassword};";

        // 👇 Put your connection string here or load from environment

        if (isTestConnectionActivated)
            optionsBuilder.UseInMemoryDatabase("TestDb");
        else
            optionsBuilder.UseMySql(connectionString,
                ServerVersion.AutoDetect(connectionString)
            );

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
