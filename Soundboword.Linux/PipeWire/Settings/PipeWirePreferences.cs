using Soundboword.Settings;

namespace Soundboword.Linux.PipeWire.Settings;

public sealed partial class PipeWirePreferences : SettingsSection
{

    public const string Key = "PipeWire";
    private const string FileName = "settings";

    public static UserData CreateData() => new(Key);

    private readonly UserData _data;

    public PipeWirePreferences() : this(CreateData())
    {
    }

    public PipeWirePreferences([FromKeyedServices(Key)] UserData data)
    {
        _data = data;
        var dto = _data.Load(FileName, () => SettingsDto.Default, SourceGenerationContext.Default.SettingsDto);
        AutoMicSounds = dto.AutoMicSounds;
    }

    public override string Title => Key;

    [ObservableProperty]
    public partial bool AutoMicSounds { get; set; }

    public override void Save() => _data.Save(FileName, new SettingsDto(AutoMicSounds), SourceGenerationContext.Default.SettingsDto);

}
