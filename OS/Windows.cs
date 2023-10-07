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
        string exeDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        string customDllRelativePath = Path.Combine(exeDirectory, "dependencies\\SDL2.dll");
        string dllDirectory = Path.GetDirectoryName(exeDirectory);

        if (!SetDllDirectory(dllDirectory))
        {
            Log.Error("Failed to set custom DLL directory.");
        }
    }
}
#endif