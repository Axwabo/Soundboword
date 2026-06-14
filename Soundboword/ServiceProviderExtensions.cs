using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Microsoft.Extensions.DependencyInjection;
using Soundboword.ViewModels;

namespace Soundboword;

public static class ServiceProviderExtensions
{

    extension(IServiceCollection collection)
    {

        public IServiceCollection AddView<TViewModel, TView>() where TViewModel : ViewModelBase, new() where TView : Control, new()
        {
            collection.AddSingleton<TViewModel>();
            return collection.AddViewLocator<TViewModel, TView>();
        }

        public IServiceCollection AddViewLocator<TViewModel, TView>() where TViewModel : ViewModelBase where TView : Control, new()
            => collection.AddSingleton<IDataTemplate>(new ViewLocator<TViewModel, TView>());

    }

}
