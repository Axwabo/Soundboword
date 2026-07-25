using System.Text.Json.Serialization;

namespace Soundboword.Sounds;

[JsonConverter(typeof(JsonStringEnumConverter<OtherSoundInteraction>))]
public enum OtherSoundInteraction
{

    Nothing,
    Stop,
    Pause,
    Mute

}
