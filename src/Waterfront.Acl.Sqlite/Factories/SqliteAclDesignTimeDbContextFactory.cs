using System.Reflection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Waterfront.Acl.Sqlite.Configuration;

namespace Waterfront.Acl.SQLite.Factories;

public class SqliteAclDesignTimeDbContextFactory : IDesignTimeDbContextFactory<SqliteAclDbContext>
{
    public SqliteAclDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().AddEnvironmentVariables("WF_ACL_SQLITE_").Build();

        SqliteAclOptions options = new SqliteAclOptions();

        configuration.Bind(options);

        if (string.IsNullOrEmpty(options.DataSource))
        {
            throw new Exception("DataSource not defined");
        }

        SqliteAclDbContext dbContext = new SqliteAclDbContext(
            new DbContextOptionsBuilder<SqliteAclDbContext>().UseSnakeCaseNamingConvention()
                                                             .UseSqlite(
                                                                 new SqliteConnectionStringBuilder {
                                                                     DataSource = options.DataSource
                                                                 }.ConnectionString,
                                                                 sqlite =>
                                                                 {
                                                                     sqlite.MigrationsAssembly(
                                                                         Assembly.GetExecutingAssembly().GetName().Name
                                                                     );
                                                                 }
                                                             )
                                                             .Options,
            options
        );

        return dbContext;
    }
}
