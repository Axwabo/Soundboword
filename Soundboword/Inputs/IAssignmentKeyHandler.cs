using Avalonia.Input;

namespace Soundboword.Inputs;

public interface IAssignmentKeyHandler
{

    void OnPressed(KeyEventArgs eventArgs, ShortcutAssigner assigner);

    void OnReleased(KeyEventArgs eventArgs, ShortcutAssigner assigner);

    void OnTextInput(TextInputEventArgs eventArgs, ShortcutAssigner assigner);

}
