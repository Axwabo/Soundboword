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
        GlobalActions.Add(ShortcutAction.StopAllSounds);
    }

    public InputsViewModel(UserData data, Lifetime lifetime, InputEditingContext context, IEnumerable<IInputFactory> factories, TabListToggles? toggles = null)
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
        foreach (var action in ShortcutAction.Global)
            if (action is not LinkToggleAction || toggles != null)
                GlobalActions.Add(action);
    }

    public List<ShortcutAction> GlobalActions { get; } = [];

    public InputEditingContext Context { get; }

    public ShortcutAssigner Assigner => Context.List.Assigner;

    public ObservableCollection<InputMethodInterface> Available { get; } = [];

    public ObservableCollection<InputMethodInterface> Unavailable { get; } = [];

    [ObservableProperty]
    public partial ShortcutAction? TargetAction { get; set; }

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
            UpdateShortcutList();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(TargetAction))
            UpdateShortcutList();
    }

    public override void OnActivated()
    {
        if (Context.Interface is { } method)
            Context.Open(method);
    }

    private void UpdateShortcutList()
    {
        if (TargetAction is not { } action || Context.Interface is not { } method)
            return;
        Assigner.Active.Clear();
        foreach (var shortcut in Context.List.For(method.Name, action))
            Assigner.Active.Add(shortcut);
    }

}
