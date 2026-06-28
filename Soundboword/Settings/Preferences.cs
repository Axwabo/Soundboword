namespace Soundboword.Settings;

public sealed partial class Preferences : ViewModelBase
{

    [ObservableProperty]
    public partial TriggerMode DefaultTriggerMode { get; set; }

}
