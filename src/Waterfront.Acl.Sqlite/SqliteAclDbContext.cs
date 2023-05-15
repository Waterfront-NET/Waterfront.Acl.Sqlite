#pragma warning disable CS8618

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Waterfront.Acl.Sqlite.Configuration;
using Waterfront.Acl.SQLite.Models;
using Waterfront.Common.Acl;
using Waterfront.Core.Parsing.Acl;

namespace Waterfront.Acl.SQLite;

public class SqliteAclDbContext : DbContext
{
    private readonly SqliteAclOptions _aclOptions;
    public DbSet<SqliteAclUser> Users { get; set; }
    public DbSet<SqliteAclPolicy> Acl { get; set; }

    public SqliteAclDbContext(
        DbContextOptions<SqliteAclDbContext> options,
        SqliteAclOptions aclOptions
    ) : base(options)
    {
        _aclOptions = aclOptions;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if ( _aclOptions.SupportsAuthentication )
        {
            modelBuilder.Entity<SqliteAclUser>()
                        .ToTable(_aclOptions.UsersTableName)
                        .HasKey(user => user.Username);
        }

        if ( _aclOptions.SupportsAuthorization )
        {
            modelBuilder.Entity<SqliteAclPolicy>()
                        .ToTable(_aclOptions.AclTableName)
                        .HasKey(acl => acl.Name);
        }
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
