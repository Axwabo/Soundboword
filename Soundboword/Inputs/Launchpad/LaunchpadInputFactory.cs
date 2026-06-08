using Soundboword.Services;

namespace Soundboword.Inputs.Launchpad;

public sealed class LaunchpadInputFactory : IInputFactory
{

    private readonly ShortcutList _shortcuts;

    public LaunchpadInputFactory(ShortcutList shortcuts) => _shortcuts = shortcuts;

    public string Name => LaunchpadInput.Name;

    public bool IsAvailable
    {
        get
        {
            foreach (var midi in AudioManager.RefreshMidiInputs())
                if (midi.Name.Contains(LaunchpadInput.Name))
                    return true;
            return false;
        }
    }

    public IInputMethod? Activate()
    {
        if (AudioManager.Midi is not { } midi)
            return null;
        foreach (var input in midi.AvailableInputs)
            if (input.Name.Contains(LaunchpadInput.Name))
                return new LaunchpadInput(midi, input, _shortcuts);
        return null;
    }

}
