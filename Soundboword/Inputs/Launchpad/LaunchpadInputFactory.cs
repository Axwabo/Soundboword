using Soundboword.Services;

namespace Soundboword.Inputs.Launchpad;

public sealed class LaunchpadInputFactory : IInputFactory
{

    private readonly SoundList _soundList;

    public LaunchpadInputFactory(SoundList soundList) => _soundList = soundList;

    public string Name => "Launchpad Mini";

    public bool IsAvailable
    {
        get
        {
            foreach (var midi in AudioManager.RefreshMidiInputs())
                if (midi.Name.Contains("Launchpad Mini"))
                    return true;
            return false;
        }
    }

    public IInputMethod? Activate()
    {
        if (AudioManager.Midi is not { } midi)
            return null;
        foreach (var input in midi.AvailableInputs)
            if (input.Name.Contains("Launchpad Mini"))
                return new LaunchpadInput(midi, input, _soundList);
        return null;
    }

}
