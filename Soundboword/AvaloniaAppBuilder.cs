using YoutubeExplode;

namespace Soundboword;

public static class AvaloniaAppBuilder
{

    public static AppBuilder Create(Action<IServiceCollection> configureServices)
        => AppBuilder.Configure(() =>
            {
                var services = new ServiceCollection()
                    .AddSoundboword()
                    .AddScoped<YoutubeClient>();
                configureServices(services);
                return new App {Services = services};
            })
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();

}
