namespace Waterfront.Acl.SQLite.Models;

public class SqliteAclUserPolicy
{
    public int Id { get; set; }
    public string Name { get; set; }
    public SqliteAclUser User { get; set; }
}