using Avalonia.Input;

namespace Soundboword.Inputs;

public interface IAssignmentKeyHandler
{

    string InputMethodName { get; }

    void OnPressed(KeyEventArgs eventArgs, ShortcutList list);

    void OnReleased(KeyEventArgs eventArgs, ShortcutList list);

    void OnTextInput(TextInputEventArgs eventArgs, ShortcutList list);

    void ResetKeys(ShortcutList list);

}
