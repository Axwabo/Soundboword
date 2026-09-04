namespace Soundboword.Settings.General;

public sealed partial class Preferences : SettingsSection
{

    private const string Filename = "general";

    public Preferences()
    {
    }

    public Preferences(UserData userData) : base(userData)
    {
        var saved = Load(() => new Preferences(), SourceGenerationContext.Default.Preferences, Filename);
        DefaultTriggerMode = saved.DefaultTriggerMode;
        DefaultInteraction = saved.DefaultInteraction;
        ConfigOnTrigger = saved.ConfigOnTrigger;
    }

    [ObservableProperty]
    public partial TriggerMode DefaultTriggerMode { get; set; }

    [ObservableProperty]
    public partial OtherSoundInteraction DefaultInteraction { get; set; }

    [ObservableProperty]
    public partial bool ConfigOnTrigger { get; set; }

    public override void Save() => Save(this, SourceGenerationContext.Default.Preferences, Filename);

}
