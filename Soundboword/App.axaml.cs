using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;
using Soundboword.Inputs;
using Soundboword.Logging;
using Soundboword.Views;

namespace Soundboword;

public sealed class App : Application
{

    public required IServiceCollection Services { get; init; }

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        Services.AddUserData(IShortcutRepository.Key)
            .AddViews()
            .AddYouTube()
            .AddLogging(builder => builder.AddAvalonia().AddFile());

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;

            var window = new MainWindow();
            Services.AddSingleton<TopLevel>(window);
            Services.AddSingleton(desktop);

            var provider = Services.BuildServiceProvider();
            DataTemplates.AddRange(provider.GetServices<IDataTemplate>());

            window.DataContext = provider.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = window;
        }
        else if (Design.IsDesignMode)
        {
            var provider = Services.BuildServiceProvider();
            DataTemplates.AddRange(provider.GetServices<IDataTemplate>());
        }

        base.OnFrameworkInitializationCompleted();
    }

}
