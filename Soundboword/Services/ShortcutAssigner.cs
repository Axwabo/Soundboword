namespace Soundboword.Services;

[RegisterSingleton(Registration = RegistrationStrategy.Self)]
public sealed partial class ShortcutAssigner : ObservableObject
{

    [ObservableProperty]
    public partial bool IsAssigning { get; set; }

    public ShortcutAction? Target { get; set; }

    public string? InputMethodFilter { get; set; }

    public List<string> EnabledInputMethods { get; } = [];

    public ObservableCollection<Shortcut> Active { get; } = [];

    public void Close()
    {
        Active.Clear();
        IsAssigning = false;
        Target = null;
        InputMethodFilter = null;
        EnabledInputMethods.Clear();
    }

    public void Update(IEnumerable<Shortcut> list, IEnumerable<string> enabledInputMethods)
    {
        Active.Clear();
        foreach (var shortcut in list)
            Active.Add(shortcut);
        EnabledInputMethods.Clear();
        EnabledInputMethods.AddRange(enabledInputMethods);
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
