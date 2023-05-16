using System.Reflection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Waterfront.Acl.Sqlite.Configuration;
using Waterfront.Acl.Sqlite.Models;
using Waterfront.Common.Authentication;
using Waterfront.Common.Tokens.Requests;
using Waterfront.Core.Authentication;

namespace Waterfront.Acl.Sqlite.Authentication;

public class SqliteAclAuthenticationService : AclAuthenticationServiceBase<SqliteAclOptions>
{
    private readonly SqliteAclDbContext _dbContext;

    public SqliteAclAuthenticationService(ILoggerFactory loggerFactory, SqliteAclOptions options) :
    base(loggerFactory, options)
    {
        if ( !Options.SupportsAuthentication )
        {
            throw new InvalidOperationException(
                $"Given {nameof(SqliteAclOptions)} instance does not support authentication"
            );
        }

        _dbContext = new SqliteAclDbContext(
            new DbContextOptionsBuilder<SqliteAclDbContext>().UseSqlite(
                                                                 new SqliteConnectionStringBuilder {
                                                                     DataSource =
                                                                     Options.UsersDataSource
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

    public override async ValueTask<AclAuthenticationResult> AuthenticateAsync(TokenRequest request)
    {
        if ( request.BasicCredentials.HasValue )
        {
            SqliteAclUser? user = await _dbContext.Users.FirstOrDefaultAsync(
                                      user => user.Username == request.BasicCredentials.Username
                                  );

            if ( user == null )
            {
                return AclAuthenticationResult.Failed(request);
            }

            if ( !BCrypt.Net.BCrypt.Verify(request.BasicCredentials.Password, user.Password) )
            {
                return AclAuthenticationResult.Failed(request);
            }

            return new AclAuthenticationResult {
                Id = request.Id,
                User =
                await _dbContext.ConvertToAclUserAsync(user)
            };
        }

        /*TODO: Refresh token credentials are not matched atm*/

        string connCrd = request.ConnectionCredentials.ToString();

        SqliteAclUser? ipMatchedUser = await _dbContext.Users.FirstOrDefaultAsync(
                                           user => !string.IsNullOrEmpty(user.IpAddress) &&
                                                   EF.Functions.Glob(connCrd, user.IpAddress)
                                       );

        if ( ipMatchedUser != null )
        {
            return new AclAuthenticationResult {
                Id   = request.Id,
                User = await _dbContext.ConvertToAclUserAsync(ipMatchedUser)
            };
        }

        SqliteAclUser? anonUser = await _dbContext.Users.FirstOrDefaultAsync(
                                      user => string.IsNullOrEmpty(user.Password) &&
                                              string.IsNullOrEmpty(user.IpAddress)
                                  );

        if ( anonUser != null )
        {
            return new AclAuthenticationResult {
                Id   = request.Id,
                User = await _dbContext.ConvertToAclUserAsync(anonUser)
            };
        }

        return AclAuthenticationResult.Failed(request);
    }
}
