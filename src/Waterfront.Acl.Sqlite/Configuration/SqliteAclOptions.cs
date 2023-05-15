using System.Diagnostics.CodeAnalysis;

namespace Waterfront.Acl.Sqlite.Configuration;

public class SqliteAclOptions
{
    public string? DataSource { get; set; }
    public SqliteAclSourceOptions? Users { get; set; }
    public SqliteAclSourceOptions? Acl { get; set; }

    [MemberNotNullWhen(true, nameof(UsersDataSource))]
    public bool SupportsAuthentication =>
    !string.IsNullOrEmpty(DataSource) || !string.IsNullOrEmpty(Users?.DataSource);

    [MemberNotNullWhen(true, nameof(AclDataSource))]
    public bool SupportsAuthorization =>
    !string.IsNullOrEmpty(DataSource) || !string.IsNullOrEmpty(Acl?.DataSource);

    public string? UsersDataSource => Users?.DataSource ?? DataSource;
    public string? AclDataSource => Acl?.DataSource ?? DataSource;

    public string UsersTableName => Users?.TableName ?? "wf_users";
    public string AclTableName => Acl?.TableName ?? "wf_acl";
}
