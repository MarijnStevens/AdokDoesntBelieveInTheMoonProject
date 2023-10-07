using System.Diagnostics;
using SDL2;

public static class Log
{
    static Stopwatch _runtimeWatch = new Stopwatch();
    static TimeSpan _lastStamp;

    public static void Init()
    {
        _runtimeWatch.Start();
    }

    [Conditional("DEBUG")]
    public static void Exit()
    {
        Console.WriteLine(_runtimeWatch.Elapsed + ": ~ " + _runtimeWatch.Elapsed.Seconds + "s");
    }

    [Conditional("DEBUG")]
    public static void Debug()
    {
        Console.WriteLine(_runtimeWatch.Elapsed + ": took " + (_runtimeWatch.Elapsed - _lastStamp).Milliseconds + "ms");
    }

    [Conditional("DEBUG")]
    public static void Debug(string message)
    {

        _lastStamp = _runtimeWatch.Elapsed;
        Console.WriteLine(_lastStamp + ": " + message);
    }

    public static void Warning(string message)
    {
        Console.WriteLine(_runtimeWatch.Elapsed + ": " + message);
    }

    public static void Error(string message)
    {
        Console.WriteLine(_runtimeWatch.Elapsed + ": " + message, SDL.SDL_GetError());
    }

#nullable enable
    public static void Error(bool getError, string message, Action? action = null)
    {
        if (getError)
        {
            Console.WriteLine(_runtimeWatch.Elapsed + ": " + message, SDL.SDL_GetError());
            action?.Invoke();
        }
    }
#nullable disable
}