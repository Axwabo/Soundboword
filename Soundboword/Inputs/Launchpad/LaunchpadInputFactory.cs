namespace Soundboword.Inputs.Launchpad;

[RegisterSingleton<IInputFactory>(Duplicate = DuplicateStrategy.Append)]
public sealed class LaunchpadInputFactory : IInputFactory
{

    private readonly SoundFlowDeviceManager _deviceManager;

    private readonly ShortcutList _shortcuts;

    public LaunchpadInputFactory(ShortcutList shortcuts, SoundFlowDeviceManager deviceManager)
    {
        _shortcuts = shortcuts;
        _deviceManager = deviceManager;
    }

    public string Name => LaunchpadInput.Name;

    public bool IsAvailable
    {
        get
        {
            foreach (var midi in _deviceManager.RefreshMidiInputs())
                if (midi.Name.Contains(LaunchpadInput.Name))
                    return true;
            return false;
        }
    }

    public Task<IInputMethod?> ActivateAsync()
    {
        if (_deviceManager.Midi is not { } midi)
            return Task.FromResult<IInputMethod?>(null);
        foreach (var input in midi.AvailableInputs)
            if (input.Name.Contains(LaunchpadInput.Name))
                return Task.FromResult<IInputMethod?>(new LaunchpadInput(midi, input, _shortcuts));
        return Task.FromResult<IInputMethod?>(null);
    }

}
