using SDL2;

public class Rect
{
  public int x;
  public int y;
  public int w;
  public int h;

  public Rect(int x, int y, int w, int h)
  {
    this.x = x;
    this.y = y;
    this.w = w;
    this.h = h;
  }

  public SDL.SDL_Rect UpdateSDL_Rect()
  {
    return new SDL.SDL_Rect
    {
      x = x,
      y = y,
      w = w,
      h = h
    };
  }
}