using System.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Waterfront.Acl.SQLite.Models;
using Waterfront.Common.Authentication;
using Waterfront.Common.Tokens;
using Waterfront.Core.Authentication;

namespace Waterfront.Acl.SQLite;

public class SqliteAclAuthenticationService : AclAuthenticationServiceBase<SqliteAclOptions>
{
    private readonly SqliteAclDbContext _dbContext;

    public SqliteAclAuthenticationService(
        ILoggerFactory loggerFactory,
        IOptions<SqliteAclOptions> options,
        SqliteAclDbContext dbContext
    ) : base(loggerFactory, options)
    {
        _dbContext = dbContext;
    }

    public override async ValueTask<AclAuthenticationResult> AuthenticateAsync(TokenRequest request)
    {
        if ( !request.BasicCredentials.IsEmpty )
        {
            SqliteAclUser? user = await _dbContext.Users.FirstOrDefaultAsync(
                                      user => user.Username == request.BasicCredentials.Username
                                  );

            if ( user == null )
            {
                return AclAuthenticationResult.FailedForRequest(request);
            }

            if ( !BCrypt.Net.BCrypt.Verify(request.BasicCredentials.Password, user.Password) )
            {
                return AclAuthenticationResult.FailedForRequest(request);
            }

            return AclAuthenticationResult.ForRequest(
                request,
                await _dbContext.ConvertToAclUserAsync(user)
            );
        }

        /*TODO: Refresh token credentials are not matched atm*/

        string connCrd = request.ConnectionCredentials.ToString();

        SqliteAclUser? ipMatchedUser = await _dbContext.Users.FirstOrDefaultAsync(
                                           user => !string.IsNullOrEmpty(user.IpAddress) &&
                                                   EF.Functions.Glob(connCrd, user.IpAddress)
                                       );

        if ( ipMatchedUser != null )
        {
            return AclAuthenticationResult.ForRequest(request, await _dbContext.ConvertToAclUserAsync(ipMatchedUser));
        }

        SqliteAclUser? anonUser = await _dbContext.Users.FirstOrDefaultAsync(
                                      user => string.IsNullOrEmpty(user.Password) &&
                                              string.IsNullOrEmpty(user.IpAddress)
                                  );

        if ( anonUser != null )
        {
            return AclAuthenticationResult.ForRequest(request, await _dbContext.ConvertToAclUserAsync(anonUser));
        }

        return AclAuthenticationResult.FailedForRequest(request);
    }
}