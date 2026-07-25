using SoundViewModel = Soundboword.Sounds.SoundViewModel;

namespace Soundboword.Inputs;

public abstract record ShortcutAction(string Id);

public sealed record TriggerSoundAction(SoundViewModel Model) : ShortcutAction(Model.Id);

public sealed record StopAllSoundsAction() : ShortcutAction("Stop All Sounds")
{

    public static StopAllSoundsAction Instance { get; } = new();

}
