using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using Soundboword.Inputs;
using Soundboword.Models;
using Soundboword.Services;

namespace Soundboword.ViewModels;

public sealed partial class InputsViewModel : ViewModelBase
{

    private static readonly string File = Path.Combine(UserData.Folder, "inputs.json");

    private readonly List<InputMethodInterface> _all;

    public ObservableCollection<InputMethodInterface> Available { get; } = [];

    public ObservableCollection<InputMethodInterface> Unavailable { get; } = [];

    public InputsViewModel() => _all = [];

    public InputsViewModel(Host host, IEnumerable<IInputFactory> factories)
    {
        _all = factories.Select(e => new InputMethodInterface(e)).ToList();
        Refresh();
        var activated = UserData.Load(File, () => new HashSet<string>());
        foreach (var input in Available)
            if (activated.Contains(input.Name))
                input.Activated = true;
        host.Lifetime.Exit += (_, _) => UserData.Save(File, _all.Where(e => e.Activated).Select(e => e.Name));
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
