#load ../data/*.cake

var mainBuildTask = Task("build");

projects.ForEach(project => {
  var task = Task(project.Task("build"))
  .IsDependentOn(project.Task("restore"))
  .Does(() => {
    DotNetBuild(project.Path.ToString(), new DotNetBuildSettings {
      Configuration = args.Configuration,
      NoRestore = true,
      NoDependencies = true
    });

    if(args.Configuration is "Release" && !args.NoCopyArtifacts && !project.IsTest) {
      Verbose("Copying build artifacts...");

      var sourceFolder = project.Directory.Combine("bin/Release/net6.0");
      var targetArchive = paths.Libraries.CombineWithFilePath($"{project.Name}.{version.SemVer}.zip");

      Zip(sourceFolder, targetArchive);
    }
  });

  project.References.ForEach(reference => task.IsDependentOn(reference.Task("build")));

  mainBuildTask.IsDependentOn(task);
});
