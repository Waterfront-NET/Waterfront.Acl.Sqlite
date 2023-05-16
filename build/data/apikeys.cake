static BuildApiKeys apikeys;

apikeys = new BuildApiKeys {
  Nuget = EnvironmentVariable("NUGET_API_KEY"),
  Github = EnvironmentVariable("GITHUB_TOKEN")
};

class BuildApiKeys {
  public string Nuget { get; set; }
  public string Github { get; set; }
}
