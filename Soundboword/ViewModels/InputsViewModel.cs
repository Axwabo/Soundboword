using Soundboword.Inputs;

namespace Soundboword.ViewModels;

public sealed partial class InputsViewModel : PageModelBase
{

    private const string File = "inputs";

    private readonly List<InputMethodInterface> _all;

    public InputsViewModel()
    {
        _all = [];
        Context = new InputEditingContext(new ShortcutList(new Lifetime(), new ShortcutAssigner(), []));
        GlobalActions.Add(ShortcutAction.StopAllSounds);
        TargetAction = GlobalActions[0];
    }

    public InputsViewModel(UserData data, Lifetime lifetime, InputEditingContext context, IEnumerable<IInputFactory> factories, TabListToggles? toggles = null)
    {
        var initial = !data.Exists(File, true);
        var prefs = data.Load(File, () => [], SourceGenerationContext.Default.IEnumerableString).ToHashSet();
        _all = factories.Select(e => new InputMethodInterface(e, context)).ToList();
        Context = context;
        Refresh();
        foreach (var method in _all)
            method.PropertyChanged += MethodOnPropertyChanged;
        foreach (var input in Available)
            if (initial || prefs.Remove(input.Name))
                input.SetActivated(true);
        lifetime.Exit += () => data.Save(File, _all.Where(e => e.Activated).Select(e => e.Name).Union(prefs), SourceGenerationContext.Default.IEnumerableString);
        context.PropertyChanged += ContextOnPropertyChanged;
        foreach (var action in ShortcutAction.Global)
            if (action is not LinkToggleAction || toggles != null)
                GlobalActions.Add(action);
        TargetAction = GlobalActions[0];
    }

    public List<ShortcutAction> GlobalActions { get; } = [];

    public InputEditingContext Context { get; }

    public ObservableCollection<InputMethodInterface> Available { get; } = [];

    public ObservableCollection<InputMethodInterface> Unavailable { get; } = [];

    [ObservableProperty]
    public partial ShortcutAction TargetAction { get; set; }

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
    private void RemoveShortcuts()
    {
        if (Context.Interface != null)
            Context.List.Remove(TargetAction);
    }

    private void ContextOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(InputEditingContext.Interface))
            UpdateShortcutList();
    }

    private void MethodOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not InputMethodInterface method || e.PropertyName != nameof(InputMethodInterface.Activated))
            return;
        var set = Context.List.Assigner.EnabledInputMethods;
        if (method.Activated)
            set.Add(method.Name);
        else
            set.Remove(method.Name);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(TargetAction))
            UpdateShortcutList();
    }

    public override void OnActivated()
    {
        if (Context.Interface is not { } method)
            return;
        Context.Open(method);
        Context.List.Assigner.Target = TargetAction;
    }

    private void UpdateShortcutList()
    {
        if (Context.Interface is not { } method)
            return;
        Context.List.Assigner.Update(Context.List.For(method.Name, TargetAction));
        Context.List.Assigner.Target = TargetAction;
    }

}
