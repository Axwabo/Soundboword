namespace Soundboword.Inputs.Launchpad;

[RegisterSingleton<IShortcutRepository>(Duplicate = DuplicateStrategy.Append)]
public sealed class LaunchpadShortcutRepository : ShortcutRepository<LaunchpadKey>
{

    public LaunchpadShortcutRepository([FromKeyedServices(IShortcutRepository.Key)] UserData data, SoundList soundList, AudioManager audioManager) : base(
        data,
        audioManager,
        soundList,
        LaunchpadInput.Name,
        e => e.FriendlyName,
        SourceGenerationContext.Default.DictionaryStringLaunchpadKey
    )
    {
    }

}
