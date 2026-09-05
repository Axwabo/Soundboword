namespace Soundboword.Linux.PipeWire.Settings;

public sealed record SettingsDto(bool AutoMicSounds, bool AutoPassthrough)
{

    public static SettingsDto Default { get; } = new(true, true);

}
