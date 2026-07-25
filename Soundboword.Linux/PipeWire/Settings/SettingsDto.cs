namespace Soundboword.Linux.PipeWire.Settings;

public sealed record SettingsDto(bool AutoMicSounds)
{

    public static SettingsDto Default { get; } = new(true);

}
