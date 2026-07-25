using System.Text.Json.Serialization;

namespace Soundboword.Sounds;

[JsonConverter(typeof(JsonStringEnumConverter<TriggerMode>))]
public enum TriggerMode
{

    StartStop,
    StartRestart,
    PlayPause,
    Duplicate

}
