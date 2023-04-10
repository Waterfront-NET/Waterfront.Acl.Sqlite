namespace Waterfront.Acl.SQLite.Models;

public class SqliteAclPolicyAccessRuleAction
{
    public int Id { get; set; }
    public string Value { get; set; }
    public SqliteAclPolicyAccessRule Rule { get; set; }
}