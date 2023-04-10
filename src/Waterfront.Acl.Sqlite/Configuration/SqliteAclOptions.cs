namespace Waterfront.Acl.SQLite;

public class SqliteAclOptions
{
    public string? DataSource { get; set; }
    public SqliteAclSourceOptions? Users { get; set; }
    public SqliteAclSourceOptions? Acl { get; set; }

    public string GetUsersDataSource()
    {
        if ( !string.IsNullOrEmpty(Users?.DataSource) )
        {
            return Users.DataSource;
        }

        if ( !string.IsNullOrEmpty(DataSource) )
            return DataSource;

        throw new Exception();
    }

    public string GetUsersTableName()
    {
        if ( !string.IsNullOrEmpty(Users?.TableName) )
            return Users.TableName;
        return "waterfront_acl_users";
    }

    public string GetAclDataSource()
    {
        if ( !string.IsNullOrEmpty(Acl?.DataSource) )
            return Acl.DataSource;

        if ( !string.IsNullOrEmpty(DataSource) )
            return DataSource;

        throw new Exception();
    }

    public string GetAclTableName()
    {
        if ( !string.IsNullOrEmpty(Acl?.TableName) )
            return Acl.TableName;

        return "waterfront_acl_policies";
    }
}