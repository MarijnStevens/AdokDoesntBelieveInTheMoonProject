using SDL2;

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
if (SDL.SDL_GetDesktopDisplayMode(0, out SDL.SDL_DisplayMode dm) == 0)
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

var renderer = SDL.SDL_CreateRenderer(window, -1,
    SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
    SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC
);

if (renderer == IntPtr.Zero)
{
    Console.WriteLine($"There was an issue creating the renderer. {SDL.SDL_GetError()}");
}

if (SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG) == 0)
{
    Console.WriteLine($"There was an issue initilizing SDL2_Image {SDL_image.IMG_GetError()}");
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

// This shit doesnt even seem to work for some reason. 
SDL.SDL_QueryTexture(imageSurface, out _, out _, out int imageWidth, out int imageHeight);

Console.WriteLine($"dest = {imageWidth}x{imageHeight}");

IntPtr texture = SDL.SDL_CreateTextureFromSurface(renderer, imageSurface);

SDL.SDL_FreeSurface(imageSurface);

SDL.SDL_Rect destRect = new SDL.SDL_Rect
{
    x = screenWidth / 2 - 830 / 2,
    y = screenHeight / 2 - 467 / 2,
    w = 830,
    h = 467
};

SDL.SDL_Event e;

// Main loop for the program
while (!quit)
{
    // Check to see if there are any events and continue to do so until the queue is empty.
    while (SDL.SDL_PollEvent(out e) != 0)
    {
        if (e.type == SDL.SDL_EventType.SDL_QUIT)
        {
            quit = true; // Exit the loop if the user closes the window
        }
        else if (e.type == SDL.SDL_EventType.SDL_KEYDOWN)
        {
            if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_ESCAPE)
            {
                quit = true; // Exit the loop if the "ESC" key is pressed
            }
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
    SDL.SDL_RenderPresent(renderer);


    // Switches out the currently presented render surface with the one we just did work on.
    SDL.SDL_RenderPresent(renderer);
}


// Clean up the resources that were created.
SDL.SDL_DestroyRenderer(renderer);
SDL.SDL_DestroyWindow(window);
SDL.SDL_Quit();