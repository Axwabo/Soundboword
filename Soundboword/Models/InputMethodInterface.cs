using Avalonia.Threading;
using Soundboword.Inputs;
using Soundboword.Inputs.Launchpad;

namespace Soundboword.Models;

public sealed partial class InputMethodInterface : ObservableObject
{

    private readonly InputEditingContext _context;

    private readonly IInputFactory _inputFactory;

    private IInputMethod? _method;

    public InputMethodInterface()
    {
        var list = new ShortcutList(null, new ShortcutAssigner());
        _inputFactory = new LaunchpadInputFactory(list, new SoundFlowDeviceManager());
        _context = new InputEditingContext(list);
    }

    public InputMethodInterface(IInputFactory inputFactory, InputEditingContext context)
    {
        _inputFactory = inputFactory;
        _context = context;
        Refresh();
    }

    public string Name => _inputFactory.Name;

    [ObservableProperty]
    public partial bool IsAvailable { get; private set; }

    [ObservableProperty]
    public partial bool Activated { get; set; }

    [ObservableProperty]
    public partial bool Toggling { get; private set; }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(Activated))
        {
            base.OnPropertyChanged(e);
            return;
        }

        _ = Activated ? ActivateAsync() : DeactivateAsync();
        base.OnPropertyChanged(e);
    }

    [RelayCommand]
    private void Refresh() => IsAvailable = _inputFactory.IsAvailable;

    [RelayCommand]
    private void Configure() => _context.Open(this);

    private async Task ActivateAsync()
    {
        Toggling = true;
        try
        {
            var method = await _inputFactory.ActivateAsync();
            _method = method;
            Dispatcher.UIThread.InvokeOrPost(() => Activated = method != null);
        }
        finally
        {
            Toggling = false;
        }
    }

    private async Task DeactivateAsync()
    {
        Toggling = true;
        try
        {
            if (_method is { } method)
                await method.DisposeAsync();
            Dispatcher.UIThread.InvokeOrPost(() => Activated = false);
        }
        finally
        {
            Toggling = false;
        }
    }

}
