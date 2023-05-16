namespace Waterfront.Acl.Sqlite.Models;

public class SqliteAclPolicy
{
    public string Name { get; set; }
    public ICollection<SqliteAclPolicyAccessRule> Access { get; set; }
}