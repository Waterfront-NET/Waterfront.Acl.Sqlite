using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Waterfront.Acl.Sqlite.Configuration;
using Waterfront.Acl.SQLite.Models;
using Waterfront.Common.Authentication;
using Waterfront.Common.Authorization;
using Waterfront.Common.Tokens;
using Waterfront.Core.Authorization;
using Waterfront.Core.Utility.Serialization.Acl;

namespace Waterfront.Acl.SQLite;

public class SqliteAclAuthorizationService : AclAuthorizationServiceBase<SqliteAclOptions>
{
    private readonly SqliteAclDbContext _dbContext;

    public SqliteAclAuthorizationService(
        ILoggerFactory loggerFactory,
        IOptions<SqliteAclOptions> options,
        SqliteAclDbContext dbContext
    ) : base(loggerFactory, options)
    {
        _dbContext = dbContext;
    }

    public override async ValueTask<AclAuthorizationResult> AuthorizeAsync(
        TokenRequest request,
        AclAuthenticationResult authnResult,
        AclAuthorizationResult authzResult
    )
    {
        if ( !authnResult.IsSuccessful )
        {
            throw new InvalidOperationException();
        }
        
        IReadOnlyList<TokenRequestScope> remainingScopes  = authzResult.ForbiddenScopes;
        List<TokenRequestScope>          authorizedScopes = new List<TokenRequestScope>();

        IQueryable<SqliteAclPolicy> userPolicies =
        _dbContext.Acl.Where(policy => authnResult.User.Acl.Contains(policy.Name));

        foreach ( TokenRequestScope scope in remainingScopes )
        {
            string strType  = scope.Type.ToSerialized();
            IQueryable<SqliteAclPolicyAccessRule> matchingAcl = userPolicies.SelectMany(acl => acl.Access)
            .Where(
                rule => rule.Type == strType &&
                        EF.Functions.Glob(scope.Name, rule.Name)
            );

            IEnumerable<string> strActions = scope.Actions.Select(a => a.ToSerialized());

            bool authorized = await matchingAcl.AnyAsync(
                                  rule => rule.Actions.Any(action => action.Value == "*") ||
                                          strActions.All(
                                              act => rule.Actions.All(act2 => act2.Value == act)
                                          )
                              );
            if ( authorized )
            {
                authorizedScopes.Add(scope);
            }
        }

        return new AclAuthorizationResult { AuthorizedScopes = authorizedScopes };
    }
}
