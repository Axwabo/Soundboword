using Soundboword.Linux.PipeWire;
using Soundboword.Linux.PipeWire.Settings;

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
        => AvaloniaAppBuilder.Create(services => services.AddSoundbowordLinux()
            .AddUserData(PipeWirePreferences.Key)
            .AddView<PipeWireView, PipeWireTabViewModel>()
            .AddView<PreferencesView, PipeWirePreferences>()
            .AddViewLocator<LinkTogglesView, LinkTogglesViewModel>()
        );

}
