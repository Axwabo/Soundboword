using Avalonia.Input;

namespace Soundboword.Inputs;

[RegisterSingleton<IAssignmentKeyHandler>]
public sealed class EphemeralKeyHandler : IAssignmentKeyHandler
{

    private static readonly Shortcut NullShortcut = new("Dummy", "", null!, true);

    private readonly ShortcutAssigner _assigner;

    public EphemeralKeyHandler(ShortcutAssigner assigner) => _assigner = assigner;

    public void OnPressed(KeyEventArgs eventArgs)
    {
    }

    public void OnReleased(KeyEventArgs eventArgs)
    {
    }

}
