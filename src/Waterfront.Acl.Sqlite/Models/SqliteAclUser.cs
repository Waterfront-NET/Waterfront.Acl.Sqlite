using System.ComponentModel.DataAnnotations;
using Waterfront.Common.Acl;

namespace Waterfront.Acl.SQLite.Models;

public class SqliteAclUser
{
    [Key]
    public string Username { get; set; }
    public string? Password { get; set; }
    public string? IpAddress { get; set; }
    public ICollection<SqliteAclUserPolicy> Acl { get; set; }
}