static BuildArgs args;

args = new BuildArgs(Context);

class BuildArgs {
  private readonly ICakeContext _context;

  public string Target => _context.Argument("target", _context.Argument("t", "build"));
  public string Configuration => _context.Argument("configuration", _context.Argument("c", "Debug"));
  public string Database => _context.Argument("database", _context.Argument("db", "wf_acl_static.db"));
  public bool NoCopyArtifacts => _context.HasArgument("no-copy-artifacts");

  public BuildArgs(ICakeContext context) => _context = context;
}
