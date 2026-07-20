namespace Soundboword.Inputs.Launchpad;

[RegisterSingleton<IShortcutRepository>(Duplicate = DuplicateStrategy.Append)]
public sealed class LaunchpadShortcutRepository : ShortcutRepository<LaunchpadKey>
{

    public LaunchpadShortcutRepository(SoundList soundList, AudioManager audioManager) : base(
        audioManager,
        soundList,
        LaunchpadInput.Name,
        e => e.FriendlyName,
        SourceGenerationContext.Default.DictionaryStringLaunchpadKey
    )
    {
    }

}
