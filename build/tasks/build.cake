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
  });

  project.References.ForEach(reference => task.IsDependentOn(reference.Task("build")));

  mainBuildTask.IsDependentOn(task);
});
