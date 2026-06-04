using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using Soundboword.Inputs;

namespace Soundboword.ViewModels;

public sealed partial class InputsViewModel : ViewModelBase
{

    private readonly List<IInputFactory> _factories;

    public ObservableCollection<IInputFactory> Factories { get; } = [];

    public InputsViewModel() => _factories = [];

    public InputsViewModel(IEnumerable<IInputFactory> factories)
    {
        _factories = factories.ToList();
        foreach (var factory in _factories)
            Factories.Add(factory);
    }

    [RelayCommand]
    private void Refresh()
    {
        Factories.Clear();
        foreach (var factory in _factories)
            Factories.Add(factory);
    }

}
