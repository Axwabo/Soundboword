using Avalonia.Controls.Templates;

namespace Soundboword;

public static class ServiceProviderExtensions
{

    extension<TView>(IServiceCollection collection) where TView : Control, new()
    {

        public IServiceCollection AddView<TViewModel>() where TViewModel : ViewModelBase, new()
        {
            collection.AddSingleton<TViewModel>();
            return collection.AddViewLocator<TView, TViewModel>();
        }

        public IServiceCollection AddViewLocator<TViewModel>() where TViewModel : ViewModelBase
            => collection.AddSingleton<IDataTemplate>(new ViewLocator<TView, TViewModel>());

    }

}
