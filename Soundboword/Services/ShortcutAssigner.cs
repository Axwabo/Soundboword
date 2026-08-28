namespace Soundboword.Services;

[RegisterSingleton(Registration = RegistrationStrategy.Self)]
public sealed partial class ShortcutAssigner : ObservableObject
{

    [ObservableProperty]
    public partial bool IsAssigning { get; set; }

    public ShortcutAction? Target { get; set; }

    public string? InputMethodFilter { get; set; }

    public ObservableCollection<Shortcut> Active { get; } = [];

    public HashSet<string> EnabledInputMethods { get; } = [];

    public void Close()
    {
        Active.Clear();
        IsAssigning = false;
        Target = null;
        InputMethodFilter = null;
    }

    public void Update(IEnumerable<Shortcut> list)
    {
        Active.Clear();
        foreach (var shortcut in list)
            Active.Add(shortcut);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName != nameof(IsAssigning) || IsAssigning)
            return;
        for (var i = Active.Count - 1; i >= 0; i--)
            if (Active[i].IsEphemeral)
                Active.RemoveAt(i);
    }

}
