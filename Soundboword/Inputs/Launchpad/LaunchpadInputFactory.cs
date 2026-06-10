using Soundboword.Services;

namespace Soundboword.Inputs.Launchpad;

public sealed class LaunchpadInputFactory : IInputFactory
{

    private readonly ShortcutList _shortcuts;
    private readonly AudioManager _audioManager;

    public LaunchpadInputFactory(ShortcutList shortcuts, AudioManager audioManager)
    {
        _shortcuts = shortcuts;
        _audioManager = audioManager;
    }

    public string Name => LaunchpadInput.Name;

    public bool IsAvailable
    {
        get
        {
            foreach (var midi in _audioManager.RefreshMidiInputs())
                if (midi.Name.Contains(LaunchpadInput.Name))
                    return true;
            return false;
        }
    }

    public IInputMethod? Activate()
    {
        if (_audioManager.Midi is not { } midi)
            return null;
        foreach (var input in midi.AvailableInputs)
            if (input.Name.Contains(LaunchpadInput.Name))
                return new LaunchpadInput(midi, input, _shortcuts);
        return null;
    }

}
