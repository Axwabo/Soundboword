using Soundboword.Settings;

namespace Soundboword.Linux.PipeWire.Settings;

public sealed partial class PipeWirePreferences : SettingsSection
{

    public const string Key = "PipeWire";

    public PipeWirePreferences()
    {
    }

    public PipeWirePreferences([FromKeyedServices(Key)] UserData data) : base(data)
    {
        var dto = Load(() => SettingsDto.Default, SourceGenerationContext.Default.SettingsDto);
        AutoMicSounds = dto.AutoMicSounds;
        AutoPassthrough = dto.AutoPassthrough;
    }

    [ObservableProperty]
    public partial bool AutoMicSounds { get; set; }

    [ObservableProperty]
    public partial bool AutoPassthrough { get; set; }

    public override void Save() => Save(new SettingsDto(AutoMicSounds, AutoPassthrough), SourceGenerationContext.Default.SettingsDto);

}
