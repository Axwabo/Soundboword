namespace Soundboword.Models;

public static class InteractionExtensions
{

    public static IReadOnlyList<OtherSoundInteraction> Interactions { get; } =
    [
        OtherSoundInteraction.Nothing,
        OtherSoundInteraction.Stop,
        OtherSoundInteraction.Pause,
        OtherSoundInteraction.Mute
    ];

}
