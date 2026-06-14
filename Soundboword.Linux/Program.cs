using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Soundboword.Linux.Services;
using Soundboword.Services;

namespace Soundboword.Linux;

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
        => AvaloniaAppBuilder.Create(services => services.AddSingleton<IFileManagerOpener, DBusFileManagerOpener>());

}
