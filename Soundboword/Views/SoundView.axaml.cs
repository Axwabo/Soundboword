using Avalonia.Input;

namespace Soundboword.Views;

public sealed partial class SoundView : UserControl
{

    public SoundView() => InitializeComponent();

    private void InputElement_OnGettingFocus(object? sender, FocusChangingEventArgs e)
    {
        if (e.NavigationMethod == NavigationMethod.Pointer)
            e.TryCancel();
    }

}
