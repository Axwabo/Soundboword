using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Soundboword.Services;
using Soundboword.ViewModels;
using Soundboword.Views;

namespace Soundboword;

public sealed class App : Application
{

    public required IServiceCollection Services { get; init; }

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        Services.AddSingleton<MainWindowViewModel>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = new MainWindow();
            Services.AddSingleton(new HostControl(window));

            var provider = Services.BuildServiceProvider();

            window.DataContext = provider.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }

}
