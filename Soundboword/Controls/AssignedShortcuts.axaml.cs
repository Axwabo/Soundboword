using System.Diagnostics.CodeAnalysis;
using Avalonia.Input;
using Soundboword.Inputs;

namespace Soundboword.Controls;

public sealed partial class AssignedShortcuts : UserControl
{

    public AssignedShortcuts() => InitializeComponent();

    private ShortcutList? List => DataContext as ShortcutList;

    private bool IsAssigning => List?.Assigner.IsAssigning ?? false;

    [MemberNotNullWhen(true, nameof(List))]
    private bool TryGetHandler([NotNullWhen(true)] out IAssignmentKeyHandler? handler)
    {
        if (List is {Assigner: {IsAssigning: true, InputMethodFilter: var filter}, KeyHandler: { } keyHandler} && (filter == null || filter == keyHandler.InputMethodName))
        {
            handler = keyHandler;
            return true;
        }

        handler = null;
        return false;
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        List?.Assigner.PropertyChanged += AssignerOnPropertyChanged;
        StartAssigning();
    }

    private void AssignerOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (IsEffectivelyVisible && sender is ShortcutAssigner && e.PropertyName == nameof(ShortcutAssigner.IsAssigning))
            StartAssigning();
    }

    private void StartAssigning()
    {
        if (!TryGetHandler(out var handler))
            return;
        Panel.Focus();
        handler.ResetKeys(List);
    }

    private void InputElement_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (TryGetHandler(out var handler))
            handler.OnPressed(e, List);
    }

    private void InputElement_OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (TryGetHandler(out var handler))
            handler.OnReleased(e, List);
    }

    private void InputElement_OnTextInput(object? sender, TextInputEventArgs e)
    {
        if (TryGetHandler(out var handler))
            handler.OnTextInput(e, List);
    }

    private void Panel_OnLosingFocus(object? sender, FocusChangingEventArgs e)
    {
        if (IsAssigning)
            e.TryCancel();
    }

}
