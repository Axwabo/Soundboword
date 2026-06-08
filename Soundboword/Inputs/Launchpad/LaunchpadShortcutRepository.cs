using Soundboword.Services;

namespace Soundboword.Inputs.Launchpad;

public sealed class LaunchpadShortcutRepository : ShortcutRepository<LaunchpadKey>
{

    public LaunchpadShortcutRepository(SoundList soundList) : base(soundList, LaunchpadInput.Name, e => e.FriendlyName)
    {
    }

}
