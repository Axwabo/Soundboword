namespace Soundboword.Editor;

[RegisterScoped(Registration = RegistrationStrategy.Self)]
public sealed class EditorContentViewModel : ViewModelBase
{

    public EditorContentViewModel(EditorContext context) => Context = context;

    public EditorContext Context { get; }

}
