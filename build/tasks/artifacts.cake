#load ../data/*.cake

var packages = GetFiles(paths.Packages.Combine("*.nupkg").ToString()).ToList();

Task("artifacts/setup-source/nuget")
.WithCriteria(!DotNetNuGetHasSource("nuget.org"))
.Does(() => {
  Information("Setting up nuget package source: nuget.org");

  DotNetNuGetAddSource("nuget.org", new DotNetNuGetSourceSettings {
    Source = "https://api.nuget.org/v3/index.json"
  });
});

Task("artifacts/push/nuget")
.WithCriteria(packages.Count is not 0)
.IsDependentOn("artifacts/setup-source/nuget")
.DoesForEach(packages, package => {
  Information("Pushing package {0} to NuGet package source", package);

  DotNetNuGetPush(package, new DotNetNuGetPushSettings {
    Source = "nuget.org",
    ApiKey = apikeys.Nuget
  });
});

Task("artifacts/setup-source/github")
.WithCriteria(!DotNetNuGetHasSource("nuget.pkg.github.com"))
.Does(() => {
  Information("Setting up nuget package source: nuget.pkg.github.com");

  DotNetNuGetAddSource("nuget.pkg.github.com", new DotNetNuGetSourceSettings {
    Source = "https://nuget.pkg.github.com/Waterfront-NET/index.json",
    UserName = "USERNAME",
    Password = apikeys.Github,
    StorePasswordInClearText = true
  });
});

Task("artifacts/push/github")
.WithCriteria(packages.Count is not 0)
.IsDependentOn("artifacts/setup-source/github")
.DoesForEach(packages, package => {
  Information("Pushing package {0} to GitHub package source", package);

  DotNetNuGetPush(package, new DotNetNuGetPushSettings {
    Source = "nuget.pkg.github.com",
    ApiKey = apikeys.Github
  });
})
