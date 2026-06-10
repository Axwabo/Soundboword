using Avalonia.Controls;
using Avalonia.Input;
using Soundboword.ViewModels;

namespace Soundboword.Views;

public partial class BoardView : UserControl
{

    public BoardView() => InitializeComponent();

    private void InputElement_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (DataContext is BoardViewModel board)
            board.Editor.Context.Close();
    }

}
