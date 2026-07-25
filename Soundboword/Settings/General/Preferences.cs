namespace Soundboword.Settings.General;

public sealed partial class Preferences : SettingsSection
{

    public override string Title => "General Settings";

    [ObservableProperty]
    public partial TriggerMode DefaultTriggerMode { get; set; }

}
