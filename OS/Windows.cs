#if Windows
using System.Runtime.InteropServices;

namespace Myriad.OS;

internal class Windows : IOperatingSystemServices
{
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool SetDllDirectory(string lpPathName);

    public void LoadDependencies()
    {
        var exeDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        var runtimeBits = Environment.Is64BitProcess ? "x64" : "x32";
        var customDllRelativePath = Path.Combine(exeDirectory, $"dependencies\\win-{runtimeBits}\\SDL2.dll");
        var dllDirectory = Path.GetDirectoryName(customDllRelativePath);

        if (!SetDllDirectory(dllDirectory))
        {
            Log.Error("Failed to set custom DLL directory.");
        }
    }
}
#endif