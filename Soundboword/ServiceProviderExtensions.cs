using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Microsoft.Extensions.DependencyInjection;
using Soundboword.ViewModels;

namespace Soundboword;

public static class ServiceProviderExtensions
{

    extension<TView>(IServiceCollection collection) where TView : Control, new()
    {

        public IServiceCollection AddView<TViewModel>() where TViewModel : ViewModelBase
        {
            collection.AddSingleton<TViewModel>();
            return collection.AddViewLocator<TView, TViewModel>();
        }

        public IServiceCollection AddViewLocator<TViewModel>() where TViewModel : ViewModelBase
            => collection.AddSingleton<IDataTemplate>(new ViewLocator<TView, TViewModel>());

    }

}
