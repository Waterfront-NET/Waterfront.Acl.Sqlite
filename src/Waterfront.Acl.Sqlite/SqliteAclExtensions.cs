using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Waterfront.Acl.Sqlite.Configuration;
using Waterfront.Common.Authentication;
using Waterfront.Common.Authorization;
using Waterfront.Extensions.DependencyInjection;

namespace Waterfront.Acl.SQLite;

public static class SqliteAclExtensions
{
    public static IWaterfrontBuilder AddSqliteAuthentication(
        this IWaterfrontBuilder builder,
        string name,
        Action<SqliteAclOptions> configureOptions
    )
    {
        builder.Services.AddScoped<IAclAuthenticationService, SqliteAclAuthenticationService>(
            services => new SqliteAclAuthenticationService(
                services.GetRequiredService<ILoggerFactory>(),
                services.GetRequiredService<IOptionsSnapshot<SqliteAclOptions>>().Get(name)
            )
        );
        builder.Services.AddSingleton<IConfigureOptions<SqliteAclOptions>>(
            new ConfigureNamedOptions<SqliteAclOptions>(name, configureOptions)
        );

        return builder;
    }

    public static IWaterfrontBuilder AddSqliteAuthentication(
        this IWaterfrontBuilder builder,
        Action<SqliteAclOptions> configureOptions
    )
    {
        return builder.AddSqliteAuthentication(Options.DefaultName, configureOptions);
    }

    public static IWaterfrontBuilder AddSqliteAuthorization(
        this IWaterfrontBuilder builder,
        string name,
        Action<SqliteAclOptions> configureOptions
    )
    {
        builder.Services.AddScoped<IAclAuthorizationService, SqliteAclAuthorizationService>(
            services => new SqliteAclAuthorizationService(
                services.GetRequiredService<ILoggerFactory>(),
                services.GetRequiredService<IOptionsSnapshot<SqliteAclOptions>>().Get(name)
            )
        );

        builder.Services.AddSingleton<IConfigureOptions<SqliteAclOptions>>(
            new ConfigureNamedOptions<SqliteAclOptions>(name, configureOptions)
        );

        return builder;
    }

    public static IWaterfrontBuilder AddSqliteAuthorization(
        this IWaterfrontBuilder builder,
        Action<SqliteAclOptions> configureOptions
    ) => builder.AddSqliteAuthorization(Options.DefaultName, configureOptions);
}
