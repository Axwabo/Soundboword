namespace Soundboword.Settings;

[RegisterSingleton<SettingsSection>(Duplicate = DuplicateStrategy.Append)]
public sealed partial class Preferences : SettingsSection
{

    public override string Title => "General Settings";

    [ObservableProperty]
    public partial TriggerMode DefaultTriggerMode { get; set; }

}
