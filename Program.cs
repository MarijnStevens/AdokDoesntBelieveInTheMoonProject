using Myriad.Myriad.OS;
using SDL2;

Log.Init();

var os = OperatingSystemServiceFactory.Create();
os.LoadDependencies();

var windowFlags =
SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP
//SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN
| SDL.SDL_WindowFlags.SDL_WINDOW_BORDERLESS
;

var renderFlags =
//SDL.SDL_RendererFlags.SDL_RENDERER_SOFTWARE;
SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED
//| SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC;
;

var displayIndex = 0;
var screen = new Rect(0, 0, 1920, 1080);
var dpi = 0f;

var windowText = "Myriad000";
float fontSizeInPoints = 24;

var benchmark = new Benchmarky();
Log.Debug("SDL_Init");
Log.Error(SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0, "There was an issue initilizing SDL.");
Log.Debug();

Log.Debug("SDL_CreateWindow");
var window = SDL.SDL_CreateWindow(windowText, 0, 0, 1920, 1080, windowFlags);
Log.Debug();

Log.Error(window == IntPtr.Zero, "There was an issue creating the window");

Log.Debug($"SDL_GetDesktopDisplayMode({displayIndex})");
if (SDL.SDL_GetDesktopDisplayMode(displayIndex, out var dm) == 0)
{
    screen.w = dm.w;
    screen.h = dm.h;
}
else
{
    Log.Debug("Unable to get desktop display mode: {}");
    SDL.SDL_Quit();
    return;
}
Log.Debug();

Log.Debug($"SDL_GetDesktopDisplayDPI({displayIndex})");
Log.Error(SDL.SDL_GetDisplayDPI(displayIndex, out dpi, out _, out _) != 0, "SDL GetDisplayDPI(" + displayIndex + ") failed. {}");
var fontSizeInPixels = fontSizeInPoints * (dpi / 72.0f);

Log.Debug("SDL_CreateRenderer");
var renderer = SDL.SDL_CreateRenderer(window, -1, renderFlags);
Log.Debug();
Log.Error(renderer == IntPtr.Zero, "There was an issue creating the renderer. {}", () => SDL.SDL_Quit());

Log.Debug("IMG_InitFlags");
Log.Error(SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG) == 0, "There was an issue initilizing SDL2_Image: {}", () => SDL.SDL_Quit());
Log.Debug();

Log.Debug("TTF_Init");
Log.Error(SDL_ttf.TTF_Init() == -1, "TTF_Init Error: {}", () => SDL.SDL_Quit());
Log.Debug();

var quit = false;

Log.Debug("IMG_Load");
var imageSurface = SDL_image.IMG_Load("assets/disclaimer.png");
Log.Debug();

if (imageSurface == IntPtr.Zero)
{
    Log.Error($"IMG_Load Error: {SDL.SDL_GetError()}");

    SDL.SDL_DestroyRenderer(renderer);
    SDL.SDL_DestroyWindow(window);
    SDL.SDL_Quit();
    return;
}

Log.Debug("SDL_QueryTexture");
SDL.SDL_QueryTexture(imageSurface, out _, out _, out var imageWidth, out var imageHeight);

Log.Debug("SDL_CreateTextureFromSurface");
var texture = SDL.SDL_CreateTextureFromSurface(renderer, imageSurface);

SDL.SDL_FreeSurface(imageSurface);

var destRect = new SDL.SDL_Rect
{
    x = (screen.w / 2) - (830 / 2),
    y = (screen.h / 2) - (467 / 2),
    w = 830,
    h = 467
};

Log.Debug("SDL_CreateTextureFromSurface(\"South_Park_Regular.ttf\")");
var font = SDL_ttf.TTF_OpenFont("assets/fonts/South_Park_Regular.ttf", (int)fontSizeInPixels);
Log.Debug();

var textColor = Color.FromArgb(255, 203, 219, 252);

var text = "FPS: 0";
var textSurface = IntPtr.Zero;
var textTexture = IntPtr.Zero;

int textWidth, textHeight;

SDL.SDL_Event e;

// Main loop for the program
Log.Debug("MainLoop");
while (!quit)
{
    while (SDL.SDL_PollEvent(out e) != 0)
    {
        if (e.type == SDL.SDL_EventType.SDL_QUIT
        || (e.type == SDL.SDL_EventType.SDL_KEYDOWN && (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_ESCAPE))
        )
        {
            quit = true;
        }
    }

    // Sets the color that the screen will be cleared with.
    SDL.SDL_SetRenderDrawColor(renderer, 17, 16, 26, 255);
    SDL.SDL_RenderClear(renderer);
    SDL.SDL_RenderCopy(renderer, texture, IntPtr.Zero, ref destRect);

    // Print FPS
    text = $"FPS: {benchmark.FPS} (Avg {benchmark.AverageFPS})";
    textSurface = SDL_ttf.TTF_RenderText_Blended(font, text, textColor);
    textTexture = SDL.SDL_CreateTextureFromSurface(renderer, textSurface);

    // regenerate texture
    SDL_ttf.TTF_SizeText(font, text, out textWidth, out textHeight);

    var fontDest = new Rect(8, 8, textWidth, textHeight);
    var fontRect = fontDest.UpdateSDL_Rect();

    SDL.SDL_RenderCopy(renderer, textTexture, IntPtr.Zero, ref fontRect);
    SDL.SDL_RenderPresent(renderer);

    benchmark.RecordFps();
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
Log.Exit();