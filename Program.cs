using System.Diagnostics;
using SDL2;

SDL.SDL_RendererFlags renderFlags = SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED
//| SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC;
;

int displayIndex = 0;

if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
{
    Console.WriteLine($"There was an issue initilizing SDL. {SDL.SDL_GetError()}");
}

IntPtr window = SDL.SDL_CreateWindow("Adok doesn't believe in the Moon Project.", 0, 0, 640, 480,
SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP);

if (window == IntPtr.Zero)
{
    Console.WriteLine($"There was an issue creating the window. {SDL.SDL_GetError()}");
}

int screenWidth, screenHeight;
if (SDL.SDL_GetDesktopDisplayMode(displayIndex, out SDL.SDL_DisplayMode dm) == 0)
{
    screenWidth = dm.w;
    screenHeight = dm.h;
}
else
{
    Console.WriteLine($"Unable to get desktop display mode: {SDL.SDL_GetError()}");
    SDL.SDL_Quit();
    return;
}

float dpi = 0f;
if (SDL.SDL_GetDisplayDPI(displayIndex, out dpi, out _, out _) != 0)
{
    Console.WriteLine($"SDL GetDisplayDPI({displayIndex}) failed.");
}

float fontSizeInPoints = 24;
float fontSizeInPixels = fontSizeInPoints * (dpi / 72.0f); // Assuming 1 inch = 72 points

var renderer = SDL.SDL_CreateRenderer(window, -1, renderFlags);

if (renderer == IntPtr.Zero)
{
    Console.WriteLine($"There was an issue creating the renderer. {SDL.SDL_GetError()}");
}

if (SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG) == 0)
{
    Console.WriteLine($"There was an issue initilizing SDL2_Image {SDL_image.IMG_GetError()}");
}

if (SDL_ttf.TTF_Init() == -1)
{
    Console.WriteLine($"TTF_Init Error: {SDL_ttf.TTF_GetError()}");
    // Handle the error accordingly
}

var quit = false;

IntPtr imageSurface = SDL_image.IMG_Load("disclaimer.png");

if (imageSurface == IntPtr.Zero)
{
    Console.WriteLine($"IMG_Load Error: {SDL.SDL_GetError()}");
    SDL.SDL_DestroyRenderer(renderer);
    SDL.SDL_DestroyWindow(window);
    SDL.SDL_Quit();
    return;
}

SDL.SDL_QueryTexture(imageSurface, out _, out _, out int imageWidth, out int imageHeight);

IntPtr texture = SDL.SDL_CreateTextureFromSurface(renderer, imageSurface);

SDL.SDL_FreeSurface(imageSurface);

SDL.SDL_Rect destRect = new SDL.SDL_Rect
{
    x = screenWidth / 2 - 830 / 2,
    y = screenHeight / 2 - 467 / 2,
    w = 830,
    h = 467
};


IntPtr font = SDL_ttf.TTF_OpenFont("fonts/South_Park_Regular.ttf", (int)fontSizeInPixels);
var textColor = Color.FromArgb(255, 203, 219, 252);

var text = "FPS: 0";
IntPtr textSurface = IntPtr.Zero;
IntPtr textTexture = IntPtr.Zero;

int textWidth, textHeight;

SDL.SDL_Event e;

int frames = 0;
long fps = 0;

double averageFPS = 0;
int previousFrameIndex = 0;
long[] previousFrames = new long[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

var stopwatch = new Stopwatch();
stopwatch.Start();

// Main loop for the program
while (!quit)
{
    while (SDL.SDL_PollEvent(out e) != 0)
    {
        if (e.type == SDL.SDL_EventType.SDL_QUIT)
        {
            quit = true;
        }
        else if (e.type == SDL.SDL_EventType.SDL_KEYDOWN && (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_ESCAPE))
        {
            quit = true;
        }
    }

    // Sets the color that the screen will be cleared with.
    if (SDL.SDL_SetRenderDrawColor(renderer, 17, 16, 26, 255) < 0)
    {
        Console.WriteLine($"There was an issue with setting the render draw color. {SDL.SDL_GetError()}");
    }

    // Clears the current render surface.
    if (SDL.SDL_RenderClear(renderer) < 0)
    {
        Console.WriteLine($"There was an issue with clearing the render surface. {SDL.SDL_GetError()}");
    }


    SDL.SDL_RenderClear(renderer);

    SDL.SDL_RenderCopy(renderer, texture, IntPtr.Zero, ref destRect);

    // Print FPS
    text = $"FPS: {fps} (Avg {averageFPS})";
    //textSurface = SDL_ttf.TTF_RenderText_Solid(font, text, textColor);
    textSurface = SDL_ttf.TTF_RenderText_Blended(font, text, textColor);

    textTexture = SDL.SDL_CreateTextureFromSurface(renderer, textSurface);
    if (SDL_ttf.TTF_SizeText(font, text, out textWidth, out textHeight) != 0)
    {
        Console.WriteLine($"TTF_SizeText Error: {SDL_ttf.TTF_GetError()}");
    }

    SDL.SDL_Rect fontDest = new SDL.SDL_Rect
    {
        x = 8,
        y = 8,
        w = textWidth,  // Width of the text
        h = textHeight  // Height of the text
    };

    SDL.SDL_RenderCopy(renderer, textTexture, IntPtr.Zero, ref fontDest);
    SDL.SDL_RenderPresent(renderer);

    frames++;
    var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

    if (elapsedMilliseconds >= 1000)
    {
        bool initAverage = false;
        if (fps == 0)
        {
            initAverage = true;
        }

        fps = frames * 1000 / elapsedMilliseconds;
        previousFrames[previousFrameIndex] = fps;

        if (initAverage)
        {
            for (int i = 0; i < previousFrames.Length; ++i)
            {
                previousFrames[i] = fps;
            }
        }

        averageFPS = previousFrames.Average();

        if (previousFrameIndex++ > previousFrames.Length)
        {
            previousFrameIndex = 0;
        }



        frames = 0;
        stopwatch.Restart();
    }
}


// Clean up the resources that were created.
SDL.SDL_DestroyRenderer(renderer);
SDL.SDL_DestroyWindow(window);

SDL.SDL_FreeSurface(textSurface);
SDL.SDL_DestroyTexture(textTexture);
SDL.SDL_DestroyRenderer(renderer);

SDL_ttf.TTF_CloseFont(font);
SDL_ttf.TTF_Quit();

SDL.SDL_Quit();