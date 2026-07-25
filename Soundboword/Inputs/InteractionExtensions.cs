using Soundboword.Sounds;

namespace Soundboword.Inputs;

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
