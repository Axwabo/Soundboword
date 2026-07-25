using Soundboword.Settings;

namespace Soundboword.Linux.PipeWire.Settings;

public sealed partial class PipeWirePreferences : SettingsSection
{

    public override string Title => "PipeWire";

    [ObservableProperty]
    public partial bool AutoMicSounds { get; set; } = true;

}
