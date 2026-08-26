using Avalonia.Input;

namespace Soundboword.Windows.GlobalHotkeys;

public static class KeyExtensions
{

    private static readonly Dictionary<Key, string> Translations = new()
    {
        {Key.Up, "Up Arrow"},
        {Key.Down, "Down Arrow"},
        {Key.Left, "Left Arrow"},
        {Key.Right, "Right Arrow"},
        {Key.Space, "Space"},
        {Key.Enter, "Enter"},
        {Key.Back, "Backspace"},
        {Key.Insert, "Insert"},
        {Key.Delete, "Delete"},
        {Key.Home, "Home"},
        {Key.End, "End"},
        {Key.PageUp, "PageUp"},
        {Key.PageDown, "PageDown"},
        {Key.Escape, "Escape"},
        {Key.CapsLock, "Caps Lock"},
        {Key.NumLock, "Num Lock"},
        {Key.NumPad0, "Numpad 0"},
        {Key.NumPad1, "Numpad 1"},
        {Key.NumPad2, "Numpad 2"},
        {Key.NumPad3, "Numpad 3"},
        {Key.NumPad4, "Numpad 4"},
        {Key.NumPad5, "Numpad 5"},
        {Key.NumPad6, "Numpad 6"},
        {Key.NumPad7, "Numpad 7"},
        {Key.NumPad9, "Numpad 9"}
        // TODO: might be missing some translations
    };

    extension(Key key)
    {

        public bool HasTranslationOverride => Translations.ContainsKey(key);

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

        public string Translate()
            => Translations.TryGetValue(eventArgs.Key, out var translation)
                ? translation
                : eventArgs.KeySymbol?.ToUpper() ?? eventArgs.Key.ToString();

    }

}
