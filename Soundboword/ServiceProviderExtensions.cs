using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls.Templates;

namespace Soundboword;

public static class ServiceProviderExtensions
{

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

}
