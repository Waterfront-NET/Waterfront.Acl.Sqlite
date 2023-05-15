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
  });

  mainPackTask.IsDependentOn(task);
});
