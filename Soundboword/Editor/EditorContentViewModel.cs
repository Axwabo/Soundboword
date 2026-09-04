namespace Soundboword.Editor;

public sealed partial class EditorContentViewModel : ViewModelBase
{

    public EditorContentViewModel() => Context = new EditorContext();

    public EditorContentViewModel(EditorContext context) => Context = context;

    public EditorContext Context { get; }

    public event Action? CloseRequested;

    [RelayCommand]
    private void SaveAndExit()
    {
        Save();
        Exit();
    }

    [RelayCommand]
    private void Save() => Context.Sound.Edits = Context.Edits;

    [RelayCommand]
    private void Exit() => CloseRequested?.Invoke();

}
