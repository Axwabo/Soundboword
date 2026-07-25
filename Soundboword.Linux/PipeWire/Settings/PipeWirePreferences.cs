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
    }

    [ObservableProperty]
    public partial bool AutoMicSounds { get; set; }

    public override void Save() => Save(new SettingsDto(AutoMicSounds), SourceGenerationContext.Default.SettingsDto);

}
