using Soundboword.Inputs;

namespace Soundboword.ViewModels;

public sealed partial class InputsViewModel : PageModelBase
{

    private const string File = "inputs";

    private readonly List<InputMethodInterface> _all;

    public InputsViewModel()
    {
        _all = [];
        Context = new InputEditingContext(new ShortcutList(null, new ShortcutAssigner()));
    }

    public InputsViewModel(UserData data, Lifetime lifetime, InputEditingContext context, IEnumerable<IInputFactory> factories)
    {
        var prefs = data.Load(File, () => [], SourceGenerationContext.Default.IEnumerableString).ToHashSet();
        _all = factories.Select(e => new InputMethodInterface(e, context)).ToList();
        Context = context;
        Refresh();
        foreach (var input in Available)
            if (prefs.Remove(input.Name))
                input.SetActivated(true);
        lifetime.Exit += () => data.Save(File, _all.Where(e => e.Activated).Select(e => e.Name).Union(prefs), SourceGenerationContext.Default.IEnumerableString);
        context.PropertyChanged += ContextOnPropertyChanged;
        ShortcutList.ShortcutsChanged += ListOnShortcutsChanged;
    }

    public InputEditingContext Context { get; }

    public ShortcutAssigner Assigner => Context.List.Assigner;

    public ObservableCollection<InputMethodInterface> Available { get; } = [];

    public ObservableCollection<InputMethodInterface> Unavailable { get; } = [];

    [ObservableProperty]
    public partial string? StopAllShortcut { get; private set; }

    [RelayCommand]
    public void Refresh()
    {
        Available.Clear();
        Unavailable.Clear();
        foreach (var method in _all)
        {
            method.Refresh();
            if (method.IsAvailable)
                Available.Add(method);
            else
                Unavailable.Add(method);
        }
    }

    [RelayCommand]
    private void RemoveShortcut()
    {
        if (Context.Interface != null)
            Context.List.Remove(ShortcutAction.StopAllSounds);
    }

    private void ContextOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(InputEditingContext.Interface))
            ListOnShortcutsChanged();
    }

    private void ListOnShortcutsChanged()
    {
        if (Context.Interface is {Name: var name})
            StopAllShortcut = Context.List.ForStopAll(name)?.FriendlyName;
    }

    public override void OnActivated()
    {
        if (Context.Interface is { } method)
            Context.Open(method);
    }

}
