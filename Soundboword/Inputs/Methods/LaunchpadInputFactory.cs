using System;

namespace Soundboword.Inputs.Methods;

public sealed class LaunchpadInputFactory : IInputFactory
{

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

    public IInputMethod? Activate() => throw new NotImplementedException();

}
