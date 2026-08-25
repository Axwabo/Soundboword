using Avalonia.Input;

namespace Soundboword.Windows.GlobalHotkeys;

public static class KeyExtensions
{

    extension(Key key)
    {

        public bool HasTranslationOverride
            => key is Key.Space
                or Key.Enter
                or Key.NumPad0
                or Key.NumPad1
                or Key.NumPad2
                or Key.NumPad3
                or Key.NumPad4
                or Key.NumPad5
                or Key.NumPad6
                or Key.NumPad7
                or Key.NumPad8
                or Key.NumPad9;

        public KeyModifiers GetModifier() => key switch
        {
            Key.LeftCtrl or Key.RightCtrl => KeyModifiers.Control,
            Key.LeftAlt or Key.RightAlt => KeyModifiers.Alt,
            Key.LeftShift or Key.RightShift => KeyModifiers.Shift,
            _ => KeyModifiers.None
        };

    }

    extension(KeyEventArgs eventArgs)
    {

        public string? Translate() => eventArgs.Key switch
        {
            Key.Up => "Up Arrow",
            Key.Down => "Down Arrow",
            Key.Left => "Left Arrow",
            Key.Right => "Right Arrow",
            Key.Space => "Space",
            Key.Enter => "Enter",
            Key.Insert => "Insert",
            Key.Delete => "Delete",
            Key.Home => "Home",
            Key.End => "End",
            Key.PageUp => "PageUp",
            Key.PageDown => "PageDown",
            Key.Escape => "Escape",
            Key.CapsLock => "Caps Lock",
            Key.NumLock => "Num Lock",
            Key.NumPad0 => "Numpad 0",
            Key.NumPad1 => "Numpad 1",
            Key.NumPad2 => "Numpad 2",
            Key.NumPad3 => "Numpad 3",
            Key.NumPad4 => "Numpad 4",
            Key.NumPad5 => "Numpad 5",
            Key.NumPad6 => "Numpad 6",
            Key.NumPad7 => "Numpad 7",
            Key.NumPad9 => "Numpad 9",
            // TODO
            _ => eventArgs.KeySymbol?.ToUpper()
        };

    }

}
