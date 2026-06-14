using Soundboword.Inputs;
using Soundboword.Inputs.Launchpad;
using YoutubeExplode;

namespace Soundboword;

public static class AvaloniaAppBuilder
{

    public static AppBuilder Create(Action<IServiceCollection> configureServices)
        => AppBuilder.Configure(() =>
            {
                var services = new ServiceCollection()
                    .AddSingleton<SoundFlowDeviceManager>()
                    .AddSingleton<AudioManager>()
                    .AddSingleton<InputEditingContext>()
                    .AddSingleton<IInputFactory, LaunchpadInputFactory>()
                    .AddSingleton<IShortcutRepository, LaunchpadShortcutRepository>()
                    .AddSingleton<FilePicker>()
                    .AddSingleton<SoundList>()
                    .AddSingleton<SoundEditingContext>()
                    .AddSingleton<ShortcutAssigner>()
                    .AddSingleton<ShortcutList>()
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
