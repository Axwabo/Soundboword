namespace Soundboword.Inputs.Launchpad;

[RegisterSingleton<IInputFactory>]
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

    public IInputMethod? Activate()
    {
        if (_deviceManager.Midi is not { } midi)
            return null;
        foreach (var input in midi.AvailableInputs)
            if (input.Name.Contains(LaunchpadInput.Name))
                return new LaunchpadInput(midi, input, _shortcuts);
        return null;
    }

}
