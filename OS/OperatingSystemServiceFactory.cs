using Myriad.OS;

namespace Myriad.Myriad.OS;

internal static class OperatingSystemServiceFactory
{
  internal static IOperatingSystemServices Create()
  {
#if Windows
    return new Windows();
#endif

#if Linux
#if OSX
  return Unix();
#endif
#endif
    throw new PlatformNotSupportedException();
  }
}
