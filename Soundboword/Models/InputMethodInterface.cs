using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Soundboword.Inputs;
using Soundboword.Inputs.Launchpad;
using Soundboword.Services;

namespace Soundboword.Models;

public sealed partial class InputMethodInterface : ObservableObject
{

    private readonly IInputFactory _inputFactory;

    private IInputMethod? _method;

    public string Name => _inputFactory.Name;

    [ObservableProperty]
    public partial bool IsAvailable { get; private set; }

    [ObservableProperty]
    public partial bool Activated { get; set; }

    public InputMethodInterface() : this(new LaunchpadInputFactory(new ShortcutList(null, new SoundList())))
    {
    }

    public InputMethodInterface(IInputFactory inputFactory)
    {
        _inputFactory = inputFactory;
        Refresh();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(Activated))
        {
            base.OnPropertyChanged(e);
            return;
        }

        if (Activated)
        {
            _method = _inputFactory.Activate();
            if (_method == null)
                Activated = false;
        }
        else
        {
            _method?.Dispose();
            _method = null;
        }

        base.OnPropertyChanged(e);
    }

    [RelayCommand]
    public void Refresh() => IsAvailable = _inputFactory.IsAvailable;

}
