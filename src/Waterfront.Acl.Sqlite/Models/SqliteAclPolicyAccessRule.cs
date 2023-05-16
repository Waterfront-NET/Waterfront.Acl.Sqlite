namespace Waterfront.Acl.Sqlite.Models;

public class SqliteAclPolicyAccessRule
{
    public int Id { get; set; }
    public SqliteAclPolicy Policy { get; set; }
    public string Type { get; set; }
    public string Name { get; set; }
    public ICollection<SqliteAclPolicyAccessRuleAction> Actions { get; set; }
}