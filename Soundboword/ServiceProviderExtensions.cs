using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls.Templates;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soundboword.Logging;
using Soundboword.Settings;
using Soundboword.Settings.General;
using Soundboword.Views;
using Soundboword.YouTube;

namespace Soundboword;

public static class ServiceProviderExtensions
{

    private static readonly Func<IServiceProvider, object?, UserData> UserDataFactory = (provider, o) => new UserData((string) o!, provider.GetRequiredService<ILoggerFactory>());

    extension<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView>(IServiceCollection collection) where TView : Control, new()
    {

        public IServiceCollection AddView<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel>() where TViewModel : ViewModelBase, new()
        {
            collection.AddSingleton<TViewModel>();
            return collection.AddViewLocator<TView, TViewModel>();
        }

        public IServiceCollection AddScopedView<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel>() where TViewModel : ViewModelBase, new()
        {
            collection.AddScoped<TViewModel>();
            return collection.AddViewLocator<TView, TViewModel>();
        }

        public IServiceCollection AddViewLocator<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel>() where TViewModel : ViewModelBase
            => collection.AddSingleton<IDataTemplate>(new ViewLocator<TView, TViewModel>());

    }

    extension(IServiceCollection collection)
    {

        public IServiceCollection AddUserData(string key)
        {
            collection.TryAdd(ServiceDescriptor.KeyedSingleton(key, UserDataFactory));
            return collection;
        }

        internal IServiceCollection AddViews() => collection.AddView<MainWindow, MainWindowViewModel>()
            .AddView<BoardView, BoardViewModel>()
            .AddView<DevicesView, DevicesViewModel>()
            .AddView<PlaybacksView, PlaybacksViewModel>()
            .AddView<InputsView, InputsViewModel>()
            .AddView<EditSoundView, EditSoundViewModel>()
            .AddView<SettingsManagerView, SettingsManager>()
            .AddView<PreferencesView, Preferences>()
            .AddViewLocator<SoundView, SoundViewModel>()
            .AddViewLocator<LogView, LogViewModel>()
            .AddViewLocator<LogListView, LogListViewModel>()
            .AddViewLocator<LogPreferencesView, LogPreferences>();

        internal IServiceCollection AddYouTube() => collection.AddScoped<AddFromYouTubeViewModel>()
            .AddScopedView<YouTubeSearchView, YouTubeSearchViewModel>()
            .AddScopedView<YouTubeVideoView, YouTubeVideoViewModel>();

    }

}
