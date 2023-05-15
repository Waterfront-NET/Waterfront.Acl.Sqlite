static BuildArgs args;

args = new BuildArgs(Context);

class BuildArgs {
  private readonly ICakeContext _context;

  public string Target => _context.Argument("target", _context.Argument("t", "build"));
  public string Configuration => _context.Argument("configuration", _context.Argument("c", "Debug"));

  public BuildArgs(ICakeContext context) => _context = context;
}
