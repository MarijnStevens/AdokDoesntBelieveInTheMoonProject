namespace Myriad.OS;

#if Linux
internal class Unix : IOperatingSystemServices
{
  public void LoadDependencies()
  {
    // Nothing to do atm.
  }
}
#endif
#if OSX
  internal class Unix : IOperatingSystemServices
  {
    public void LoadDependencies()
    {
      // Nothing to do atm.
    }
  }
#endif
