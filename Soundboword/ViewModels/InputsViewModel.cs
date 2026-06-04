using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using Soundboword.Inputs;
using Soundboword.Models;

namespace Soundboword.ViewModels;

public sealed partial class InputsViewModel : ViewModelBase
{

    private readonly List<InputMethodInterface> _all;

    public ObservableCollection<InputMethodInterface> Available { get; } = [];

    public ObservableCollection<InputMethodInterface> Unavailable { get; } = [];

    public InputsViewModel() => _all = [];

    public InputsViewModel(IEnumerable<IInputFactory> factories)
    {
        _all = factories.Select(e => new InputMethodInterface(e)).ToList();
        Refresh();
    }

    [RelayCommand]
    private void Refresh()
    {
        Available.Clear();
        Unavailable.Clear();
        foreach (var method in _all)
            if (method.IsAvailable)
                Available.Add(method);
            else
                Unavailable.Add(method);
    }

}
