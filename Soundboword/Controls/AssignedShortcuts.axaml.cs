using Avalonia.Input;

namespace Soundboword.Controls;

public sealed partial class AssignedShortcuts : UserControl
{

    public AssignedShortcuts() => InitializeComponent();

    private ShortcutList? List => DataContext as ShortcutList;

    private bool IsAssigning => List?.Assigner.IsAssigning ?? false;

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        List?.Assigner.PropertyChanged += AssignerOnPropertyChanged;
    }

    private void AssignerOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (!IsEffectivelyVisible || sender is not ShortcutAssigner assigner || e.PropertyName != nameof(ShortcutAssigner.IsAssigning))
            return;
        if (assigner.IsAssigning)
            Panel.Focus();
    }

    private void InputElement_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (IsAssigning)
            List?.KeyHandler?.OnPressed(e, List.Assigner);
    }

    private void InputElement_OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (IsAssigning)
            List?.KeyHandler?.OnReleased(e, List.Assigner);
    }

    private void InputElement_OnTextInput(object? sender, TextInputEventArgs e)
    {
        if (IsAssigning)
            List?.KeyHandler?.OnTextInput(e, List.Assigner);
    }

}
