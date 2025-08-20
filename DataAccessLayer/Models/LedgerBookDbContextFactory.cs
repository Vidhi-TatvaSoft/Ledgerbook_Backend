using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DataAccessLayer.Models;

public class LedgerBookDbContextFactory : IDesignTimeDbContextFactory<LedgerBookDbContext>
{

    public LedgerBookDbContext CreateDbContext(string[] args)
    {

        string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        string basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "LedgerBook");

        IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

        string connectionString = config.GetConnectionString("LedgerbookDbConnection");

        DbContextOptionsBuilder<LedgerBookDbContext> optionsBuilder = new DbContextOptionsBuilder<LedgerBookDbContext>();
        optionsBuilder.UseSqlServer(connectionString);
        return new LedgerBookDbContext(optionsBuilder.Options);

    }
}
