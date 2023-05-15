static BuildPaths paths;
paths = new BuildPaths { Root = Context.Environment.WorkingDirectory };

class BuildPaths {
  public DirectoryPath Root { get; set; }
  public DirectoryPath Src => Root.Combine("src");
  public DirectoryPath Test => Root.Combine("test");
  public FilePath Solution => Root.CombineWithFilePath("Waterfront.Acl.Sqlite.sln");
  public DirectoryPath Libraries => Root.Combine("artifacts/lib");
  public DirectoryPath Packages => Root.Combine("artifacts/pkg");
}
