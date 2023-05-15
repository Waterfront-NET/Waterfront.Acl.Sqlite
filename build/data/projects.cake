#addin "nuget:?package=Cake.Incubator&version=8.0.0"
#load paths.cake
#load args.cake

static List<BuildProject> projects;

projects = ParseSolution(paths.Solution).Projects
.Where(solutionProject => solutionProject.IsType(ProjectType.CSharp))
.Select(solutionProject => {
  var fullProjectPath = solutionProject.Path.MakeAbsolute(paths.Solution.GetDirectory());
  var projectParseResult = ParseProject(fullProjectPath, args.Configuration);

  var buildProject = new BuildProject {
    Name = solutionProject.Name,
    Path = fullProjectPath,
    IsTest = projectParseResult.IsTestProject()
  };

  projectParseResult.ProjectReferences.ToList().ForEach(projectReference => {
    var fullReferencePath = projectReference.FilePath.MakeAbsolute(fullProjectPath.GetDirectory());

    buildProject.References.Add(new BuildProject {
      Name = fullReferencePath.GetFilenameWithoutExtension().ToString(),
      Path = fullReferencePath
    });
  });

  return buildProject;
})
.ToList();

class BuildProject {
  public string Name { get; set; }
  public string Shortname => Name.Replace("Waterfront.", string.Empty).ToLowerInvariant();
  public FilePath Path { get; set; }
  public bool IsTest { get; set; }
  public DirectoryPath Directory => Path.GetDirectory();
  public DirectoryPath BinDirectory => Directory.Combine("bin");
  public List<BuildProject> References { get; }

  public BuildProject() {
    References = new List<BuildProject>();
  }

  public DirectoryPath Bin(string configuration) {
    return BinDirectory.Combine(configuration);
  }

  public DirectoryPath Bin(string configuration, string framework) {
    return Bin(configuration).Combine(framework);
  }

  public FilePath Nupkg(string configuration, string version) {
    return Bin(configuration).CombineWithFilePath($"{Name}.{version}.nupkg");
  }

  public FilePath Snupkg(string configuration, string version) {
    return Bin(configuration).CombineWithFilePath($"{Name}.{version}.snupkg");
  }

  public string Task(string name) {
    return $":{Shortname}:{name}";
  }
}
