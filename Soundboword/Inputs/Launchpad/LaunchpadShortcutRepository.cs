using Soundboword.Services;

namespace Soundboword.Inputs.Launchpad;

public sealed class LaunchpadShortcutRepository : ShortcutRepository<LaunchpadKey>
{

    public LaunchpadShortcutRepository(SoundList soundList, AudioManager audioManager) : base(audioManager, soundList, LaunchpadInput.Name, e => e.FriendlyName)
    {
    }

}
