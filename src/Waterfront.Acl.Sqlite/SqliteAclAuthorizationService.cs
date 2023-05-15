using System.Data;
using System.Reflection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Waterfront.Acl.Sqlite.Configuration;
using Waterfront.Acl.SQLite.Models;
using Waterfront.Common.Authentication;
using Waterfront.Common.Authorization;
using Waterfront.Common.Tokens.Requests;
using Waterfront.Core.Authorization;
using Waterfront.Core.Serialization.Acl;

namespace Waterfront.Acl.SQLite;

public class SqliteAclAuthorizationService : AclAuthorizationServiceBase<SqliteAclOptions>
{
    private readonly SqliteAclDbContext _dbContext;

    public SqliteAclAuthorizationService(ILoggerFactory loggerFactory, SqliteAclOptions options) :
    base(loggerFactory, options)
    {
        if ( !Options.SupportsAuthorization )
        {
            throw new InvalidOperationException(
                $"Given {nameof(SqliteAclOptions)} instance does not support authorization"
            );
        }

        _dbContext = new SqliteAclDbContext(
            new DbContextOptionsBuilder<SqliteAclDbContext>().UseSqlite(
                                                                 new SqliteConnectionStringBuilder {
                                                                     DataSource =
                                                                     Options.AclDataSource
                                                                 }
                                                                 .ConnectionString,
                                                                 sqlite => {
                                                                     sqlite.MigrationsAssembly(
                                                                         Assembly
                                                                         .GetExecutingAssembly()
                                                                         .GetName()
                                                                         .Name
                                                                     );
                                                                 }
                                                             )
                                                             .UseSnakeCaseNamingConvention()
                                                             .Options,
            Options
        );
    }

    public override async ValueTask<AclAuthorizationResult> AuthorizeAsync(
        TokenRequest request,
        AclAuthenticationResult authnResult,
        AclAuthorizationResult authzResult
    )
    {
        if ( !authnResult.IsSuccessful )
        {
            return new AclAuthorizationResult { ForbiddenScopes = request.Scopes };
        }

        var user = authnResult.User;

        var result = new AclAuthorizationResult {
            AuthorizedScopes = new List<TokenRequestScope>(),
            ForbiddenScopes =
            new List<TokenRequestScope>()
        };

        List<TokenRequestScope> authorizedScopes = new List<TokenRequestScope>();
        List<TokenRequestScope> forbiddenScopes  = new List<TokenRequestScope>();

        var policies = _dbContext.Acl
                                 .Where(
                                     p => user.Acl.Contains(
                                         p.Name,
                                         StringComparer.OrdinalIgnoreCase
                                     )
                                 )
                                 .Include(p => p.Access)
                                 .ThenInclude(a => a.Actions)
                                 .ToArrayAsync();

        var userPolicies = _dbContext.Acl.Where(
            p => user.Acl.Contains(p.Name, StringComparer.OrdinalIgnoreCase)
        );

        foreach ( TokenRequestScope remainingScope in authzResult.ForbiddenScopes )
        {
            var strScopeType = remainingScope.Type.ToSerialized();

            var matchingAcl = userPolicies.SelectMany(p => p.Access)
                                          .Where(
                                              rule => rule.Type == strScopeType &&
                                                      EF.Functions.Glob(
                                                          remainingScope.Name,
                                                          rule.Name
                                                      )
                                          );

            var strScopeActions = remainingScope.Actions.Select(x => x.ToSerialized());

            bool isAuthorized = await matchingAcl.AnyAsync(
                                    x => x.Actions.Any(y => y.Value == "*") ||
                                         strScopeActions.All(z => x.Actions.Any(r => r.Value == z))
                                );

            if ( isAuthorized )
            {
                result.AuthorizedScopes.Add(remainingScope);
            }
            else
            {
                result.ForbiddenScopes.Add(remainingScope);
            }
        }

        return result;
    }
}
