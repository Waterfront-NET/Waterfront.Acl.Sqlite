#load ../data/*.cake

var mainPackTask = Task("pack");

projects.Where(project => !project.IsTest).ToList().ForEach(project => {
  var task = Task(project.Task("pack"))
  .IsDependentOn(project.Task("build"))
  .WithCriteria(args.Configuration is "Release")
  .Does(() => {
    DotNetPack(project.Path.ToString(), new DotNetPackSettings {
      Configuration = args.Configuration,
      NoBuild = true
    });

    if(args.Configuration is "Release" && !args.NoCopyArtifacts) {
      Verbose("Copying built packages to artifacts directory...");

      var sourceFolder = project.Directory.Combine("bin/Release");
      GetFiles(sourceFolder.Combine($"{project.Name}.{version.SemVer}.{{nupkg,snupkg}}").ToString())
      .ToList()
      .ForEach(packagePath => {
        Verbose("Copying package {0}...", packagePath);
        CopyFileToDirectory(packagePath, paths.Packages);
      });
    }
  });

  mainPackTask.IsDependentOn(task);
});
