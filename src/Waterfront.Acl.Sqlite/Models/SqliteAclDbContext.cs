#pragma warning disable CS8618

using Microsoft.EntityFrameworkCore;
using Waterfront.Acl.Sqlite.Configuration;
using Waterfront.Common.Acl;
using Waterfront.Core.Parsing.Acl;

namespace Waterfront.Acl.Sqlite.Models;

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
            modelBuilder.Entity<SqliteAclUserPolicy>().ToTable("wf_acl_user_policies");
        }

        if ( _aclOptions.SupportsAuthorization )
        {
            modelBuilder.Entity<SqliteAclPolicy>()
                        .ToTable(_aclOptions.AclTableName)
                        .HasKey(acl => acl.Name);

            modelBuilder.Entity<SqliteAclPolicyAccessRule>().ToTable("wf_acl_policy_access_rules");
            modelBuilder.Entity<SqliteAclPolicyAccessRuleAction>()
                        .ToTable("wf_acl_policy_access_rule_actions");
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
