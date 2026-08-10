using Avalonia.Input;

namespace Soundboword.Inputs;

public interface IAssignmentKeyHandler
{

    void OnPressed(KeyEventArgs eventArgs);

    void OnReleased(KeyEventArgs eventArgs);

    void OnTextInput(TextInputEventArgs eventArgs);

}
