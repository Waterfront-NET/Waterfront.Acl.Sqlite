#load build/tasks/*.cake
#load build/data/args.cake
#load build/data/version.cake

Setup(ctx => {

  EnsureDirectoryExists(paths.Libraries);
  EnsureDirectoryExists(paths.Packages);

  Environment.SetEnvironmentVariable("SEMANTIC_VERSION", version.SemVer);
  Environment.SetEnvironmentVariable("INFO_VERSION", version.InformationalVersion);
});

RunTarget(args.Target);
