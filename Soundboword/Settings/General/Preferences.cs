namespace Soundboword.Settings.General;

public sealed partial class Preferences : SettingsSection
{

    [ObservableProperty]
    public partial TriggerMode DefaultTriggerMode { get; set; }

    [ObservableProperty]
    public partial OtherSoundInteraction DefaultInteraction { get; set; }

    public override void Save()
    {
    }

}
