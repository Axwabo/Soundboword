namespace Soundboword.Models;

public abstract record ShortcutAction(string Id)
{

    public static StopAllSoundsAction StopAllSounds { get; } = new();

    public static LinkToggleAction ToggleHearSounds { get; } = new(LinkToggleAction.HearSounds);
    public static LinkToggleAction ToggleMicSounds { get; } = new(LinkToggleAction.MicSounds);
    public static LinkToggleAction ToggleMicPassthrough { get; } = new(LinkToggleAction.MicPassthrough);
    public static LinkToggleAction ToggleHearMyself { get; } = new(LinkToggleAction.HearMyself);

    public static IReadOnlyList<ShortcutAction> Global { get; } = [StopAllSounds, ToggleHearSounds, ToggleMicSounds, ToggleMicPassthrough, ToggleHearMyself];

}

public sealed record TriggerSoundAction(SoundViewModel Model) : ShortcutAction(Model.Id);

public sealed record StopAllSoundsAction() : ShortcutAction("Stop All Sounds");

public sealed record LinkToggleAction(string Id) : ShortcutAction(Id)
{

    public const string HearSounds = "Hear Sounds";

    public const string MicSounds = "Mic Sounds";

    public const string MicPassthrough = "Mic Passthrough";

    public const string HearMyself = "Hear Myself";

}
