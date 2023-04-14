﻿using System.ComponentModel.DataAnnotations;

namespace Waterfront.Acl.SQLite.Models;

public class SqliteAclPolicy
{
    public string Name { get; set; }
    public ICollection<SqliteAclPolicyAccessRule> Access { get; set; }
}