using Avalonia.Input;

namespace Soundboword.Controls;

public sealed partial class AssignedShortcuts : UserControl
{

    public AssignedShortcuts() => InitializeComponent();

    private ShortcutAssignmentContext? Context => DataContext as ShortcutAssignmentContext;

    private bool IsAssigning => Context?.List.Assigner.IsAssigning ?? false;

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        Context?.List.Assigner.PropertyChanged += AssignerOnPropertyChanged;
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
            Context?.KeyHandler?.OnPressed(e);
    }

    private void InputElement_OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (IsAssigning)
            Context?.KeyHandler?.OnReleased(e);
    }

    private void InputElement_OnTextInput(object? sender, TextInputEventArgs e)
    {
        if (IsAssigning)
            Context?.KeyHandler?.OnTextInput(e);
    }

}
