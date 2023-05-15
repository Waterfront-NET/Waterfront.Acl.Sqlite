using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Waterfront.Acl.Sqlite.Configuration;

namespace Waterfront.Acl.SQLite.Factories;

public class SqliteAclDesignTimeDbContextFactory : IDesignTimeDbContextFactory<SqliteAclDbContext>
{
    public SqliteAclDbContext CreateDbContext(string[] args)
    {
        var configurationBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.Design.json", false);

        var configuration = configurationBuilder.Build();

        var connectionString = configuration.GetConnectionString("Database");

        var dbContext = new SqliteAclDbContext(
            new DbContextOptionsBuilder<SqliteAclDbContext>()
            .UseSnakeCaseNamingConvention()
            .UseSqlite(connectionString,
                       sqlite => {
                           sqlite.MigrationsAssembly(
                               Assembly.GetExecutingAssembly().GetName().Name
                           );
                       })
            .Options,
            new SqliteAclOptions { }
        );

        return dbContext;
    }
}
