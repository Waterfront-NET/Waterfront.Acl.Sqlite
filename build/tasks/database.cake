#load ../data/version.cake
#load ../data/projects.cake
#load ../data/args.cake

var project = projects.Find(p => p.Name == "Waterfront.Acl.Sqlite");
var envVars = new Dictionary<string,string> {
  {"WF_ACL_SQLITE_DATASOURCE", args.Database}
};

Task("db/add-migration")
.Does(() => {
  var migrationVersion = version.SemVer;

  StartProcess("dotnet", new ProcessSettings {
    Arguments = $"ef migrations add {migrationVersion}",
    WorkingDirectory = project.Path.GetDirectory(),
    EnvironmentVariables = envVars
  });
});

Task("db/remove-migration")
.Does(() => {
  StartProcess("dotnet", new ProcessSettings {
    Arguments = "ef migrations remove",
    WorkingDirectory = project.Path.GetDirectory(),
    EnvironmentVariables = envVars
  });
});

Task("db/list-migrations")
.Does(() => {
  var migrations = new List<string>();
  var failed = false;

  StartProcess("dotnet", new ProcessSettings {
    Arguments = "ef migrations list --project " + project.Path.ToString(),
    RedirectStandardOutput = true,
    RedirectedStandardOutputHandler = log => {
      if(log != null && log is not "Build started..." and not "Build succeeded.") {
        migrations.Add(log);
      }

      return log;
    },
    RedirectStandardError = true,
    RedirectedStandardErrorHandler = log => {
      if(log != null) {
        failed = true;
        Error(log);
      }
      return log;
    },
    EnvironmentVariables = envVars,
    WorkingDirectory = project.Path.GetDirectory()
  });

  if(failed) {
    throw new CakeException("Failed dotnet ef command");
  }

  migrations.ForEach(migration => Information("Migration found: {0}", migration));

  // Information("Migrations:\n{0}", string.Join("\n", migrations));
});
