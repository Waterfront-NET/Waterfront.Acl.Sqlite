[CmdletBinding()]
param(
  $database_name
)

$env:WF_ACL_SQLITE_DataSource = $database_name ?? 'wf_acl.db'

dotnet ef database update
