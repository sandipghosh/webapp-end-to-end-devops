
using System;
using Xunit;
using dataaccess;

namespace dataaccess.tests;

public class ApplicationDbContextFactoryTests
{
    [Fact]
    public void CreateDbContext_Returns_Context()
    {
        // Arrange
        Environment.SetEnvironmentVariable("WEBAPP_DB_HOST", "127.0.0.1");
        Environment.SetEnvironmentVariable("WEBAPP_DB_PORT", "3306");
        Environment.SetEnvironmentVariable("WEBAPP_DB_NAME", "app_db");
        Environment.SetEnvironmentVariable("WEBAPP_DB_USER", "sandip");
        Environment.SetEnvironmentVariable("WEBAPP_DB_PASSWORD", "sandev@1984");

        var factory = new ApplicationDbContextFactory();

        // Act
        var ctx = factory.CreateDbContext(Array.Empty<string>());

        // Assert
        Assert.NotNull(ctx);
        Assert.IsType<ApplicationDbContext>(ctx);
        // Provider name should be set when options are configured
        Assert.False(string.IsNullOrWhiteSpace(ctx.Database.ProviderName));
    }
}