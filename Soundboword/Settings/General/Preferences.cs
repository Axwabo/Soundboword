namespace Soundboword.Settings.General;

public sealed partial class Preferences : SettingsSection
{

    [ObservableProperty]
    public partial TriggerMode DefaultTriggerMode { get; set; }

    public override void Save()
    {
    }

}
