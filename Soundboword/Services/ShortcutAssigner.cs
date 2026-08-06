namespace Soundboword.Services;

[RegisterSingleton(Registration = RegistrationStrategy.Self)]
public sealed partial class ShortcutAssigner : ObservableObject
{

    [ObservableProperty]
    public partial bool IsAssigning { get; set; }

    public ShortcutAction? Target { get; set; }

    public string? InputMethodFilter { get; set; }

    public ObservableCollection<Shortcut> Active { get; } = [];

    public void Close()
    {
        Active.Clear();
        IsAssigning = false;
        Target = null;
        InputMethodFilter = null;
    }

}
