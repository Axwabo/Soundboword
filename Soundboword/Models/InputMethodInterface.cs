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
        _inputFactory = new LaunchpadInputFactory(list, new SoundFlowDeviceManager(new UserData()));
        _context = new InputEditingContext(list);
    }

    public InputMethodInterface(IInputFactory inputFactory, InputEditingContext context)
    {
        _inputFactory = inputFactory;
        _context = context;
    }

    public string Name => _inputFactory.Name;

    [ObservableProperty]
    public partial bool IsAvailable { get; private set; }

    [ObservableProperty]
    public partial bool Activated { get; private set; }

    [ObservableProperty]
    public partial bool ActivationPending { get; private set; }

    [RelayCommand]
    private void Toggle()
    {
        if (ActivationPending)
            return;
        if (!Activated)
            _ = ActivateAsync();
        else
        {
            _method?.Dispose();
            _method = null;
        }
    }

    public void SetActivated(bool activated)
    {
        if (activated != Activated)
            Toggle();
    }

    [RelayCommand]
    public void Refresh() => IsAvailable = _inputFactory.IsAvailable;

    [RelayCommand]
    private void Configure() => _context.Open(this);

    private async Task ActivateAsync()
    {
        if (ActivationPending)
            return;
        ActivationPending = true;
        try
        {
            var method = await _inputFactory.ActivateAsync();
            _method = method;
            Dispatcher.UIThread.InvokeOrPost(() => Activated = method != null);
        }
        finally
        {
            ActivationPending = false;
        }
    }

}
