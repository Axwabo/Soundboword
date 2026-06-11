using System;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Soundboword.Inputs;
using Soundboword.Inputs.Launchpad;
using Soundboword.Services;

namespace Soundboword;

internal static class Program
{

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure(() => new App
            {
                Services = new ServiceCollection()
                    .AddSingleton<AudioManager>()
                    .AddSingleton<IInputFactory, LaunchpadInputFactory>()
                    .AddSingleton<IShortcutRepository, LaunchpadShortcutRepository>()
                    .AddSingleton<IFileManagerOpener, DBusFileManagerOpener>()
                    .AddSingleton<FilePicker>()
                    .AddSingleton<SoundList>()
                    .AddSingleton<SoundEditingContext>()
                    .AddSingleton<ShortcutAssigner>()
                    .AddSingleton<ShortcutList>()
            })
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();

}
