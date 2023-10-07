#if Linux
#if OSX
namespace Myriad.Myriad.OS
{
  internal class Unix : IOperatingSystemServices
  {
    public void LoadDependencies()
    {
      // Nothing to do atm.
    }
  }
}
#endif
#endif