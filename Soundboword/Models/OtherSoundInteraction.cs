using System.Text.Json.Serialization;

namespace Soundboword.Models;

[JsonConverter(typeof(JsonStringEnumConverter<OtherSoundInteraction>))]
public enum OtherSoundInteraction
{

    Nothing,
    Stop,
    Pause,
    Mute

}
