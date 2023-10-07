
using SDL2;

public static class Color
{
    public static SDL.SDL_Color FromArgb(byte a, byte r, byte g, byte b)
    {
        return new SDL.SDL_Color()
        {
            a = a,
            r = r,
            g = g,
            b = b,
        };
    }
}