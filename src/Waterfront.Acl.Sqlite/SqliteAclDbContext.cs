using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Waterfront.Acl.Sqlite.Configuration;
using Waterfront.Acl.SQLite.Models;
using Waterfront.Common.Acl;
using Waterfront.Core.Utility.Parsing.Acl;

namespace Waterfront.Acl.SQLite;

public class SqliteAclDbContext : DbContext
{
    private readonly IOptions<SqliteAclOptions> _options;

    public DbSet<SqliteAclUser> Users { get; set; }
    public DbSet<SqliteAclPolicy> Acl { get; set; }

    public SqliteAclDbContext(IOptions<SqliteAclOptions> options)
    {
        _options = options;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var usersTableName = _options.Value.GetUsersTableName();
        var aclTableName   = _options.Value.GetAclTableName();

        modelBuilder.Entity<SqliteAclUser>().ToTable(usersTableName);
        modelBuilder.Entity<SqliteAclPolicy>().ToTable(aclTableName);

        modelBuilder.Entity<SqliteAclUser>().HasKey(user => user.Username);
        modelBuilder.Entity<SqliteAclPolicy>().HasKey(policy => policy.Name);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // var dataSource     = _options.Value.DataSource!;
        var dataSource = "auth.db";
        optionsBuilder.UseSqlite(
            new SqliteConnectionStringBuilder { DataSource = dataSource }.ConnectionString,
            opt => opt.MigrationsAssembly("Waterfront.Server")
        );
    }

    public async ValueTask<AclUser> ConvertToAclUserAsync(SqliteAclUser sqliteAclUser)
    {
        string[] acl = await Entry(sqliteAclUser)
                             .Collection(user => user.Acl)
                             .Query()
                             .Select(acl => acl.Name)
                             .ToArrayAsync();
        return new AclUser {
            Username = sqliteAclUser.Username,
            Acl      = acl
        };
    }

    public async ValueTask<AclPolicy> ConvertToAclPolicyAsync(SqliteAclPolicy sqliteAclPolicy)
    {
        SqliteAclPolicyAccessRule[] access = await Entry(sqliteAclPolicy)
                                                   .Collection(a => a.Access)
                                                   .Query()
                                                   .Include(x => x.Actions)
                                                   .ToArrayAsync();

        return new AclPolicy {
            Name = sqliteAclPolicy.Name,
            Access = access.Select(
                a => new AclAccessRule {
                    Name = a.Name,
                    Type = AclEntityParser.ParseResourceType(a.Type),
                    Actions = a.Actions.Select(
                        action => AclEntityParser.ParseResourceAction(action.Value)
                    )
                }
            )
        };
    }
}
