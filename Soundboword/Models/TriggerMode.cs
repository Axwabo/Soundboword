using System.Text.Json.Serialization;

namespace Soundboword.Models;

[JsonConverter(typeof(JsonStringEnumConverter<TriggerMode>))]
public enum TriggerMode
{

    StartStop,
    StartRestart,
    PlayPause,
    Duplicate

}
