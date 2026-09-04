namespace Soundboword.Editor;

public sealed class EditorContentViewModel : ViewModelBase
{

    public EditorContentViewModel() => Context = new EditorContext();

    public EditorContentViewModel(EditorContext context) => Context = context;

    public EditorContext Context { get; }

}
