namespace Myriad.OS;

internal static class OperatingSystemServiceFactory
{
  internal static IOperatingSystemServices Create()
  {
#if Windows
    return new Windows();
#endif
#if Linux
    return new Unix();
#endif
#if OSX
    return new Unix();
#endif
    throw new PlatformNotSupportedException();
  }
}