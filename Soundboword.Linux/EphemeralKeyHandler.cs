#if DEBUG
using System.Text;
using Avalonia.Input;
using Soundboword.Inputs;

namespace Soundboword.Linux;

[RegisterSingleton<IAssignmentKeyHandler>]
public sealed class EphemeralKeyHandler : IAssignmentKeyHandler
{

    private static readonly Shortcut NullShortcut = new("Dummy", "", null!, true);

    private static KeyModifiers GetModifier(Key key) => key switch
    {
        Key.LeftCtrl or Key.RightCtrl => KeyModifiers.Control,
        Key.LeftAlt or Key.RightAlt => KeyModifiers.Alt,
        Key.LeftShift or Key.RightShift => KeyModifiers.Shift,
        _ => KeyModifiers.None
    };

    private Key _lastKey;

    private string _lastSymbol = "";

    private KeyModifiers _modifiers;

    public void OnPressed(KeyEventArgs eventArgs, ShortcutList list)
    {
        var modifier = GetModifier(eventArgs.Key);
        if (modifier != KeyModifiers.None)
            _modifiers |= modifier;
        else
            _lastKey = eventArgs.Key;
        var symbol = eventArgs.Key switch
        {
            Key.Up => "Up Arrow",
            Key.Down => "Down Arrow",
            Key.Left => "Left Arrow",
            Key.Right => "Right Arrow",
            Key.Space => "Space",
            Key.Enter => "Enter",
            // TODO: localization, cuz apparently this isn't localized :sob:
            _ => eventArgs.KeySymbol?.ToUpper()
        };
        if (!string.IsNullOrEmpty(symbol))
            _lastSymbol = symbol;
        Update(list.Assigner);
    }

    public void OnReleased(KeyEventArgs eventArgs, ShortcutList list)
    {
        var modifier = GetModifier(eventArgs.Key);
        if (modifier != KeyModifiers.None)
        {
            _modifiers &= ~modifier;
            Update(list.Assigner);
        }
        else if (_lastKey == eventArgs.Key)
            Update(list.Assigner, true);
        else
            Update(list.Assigner);
    }

    public void OnTextInput(TextInputEventArgs eventArgs, ShortcutList list)
    {
        _lastSymbol = _lastKey switch
        {
            Key.Space => "Space",
            Key.Enter => "Enter",
            _ => eventArgs.Text?.ToUpper() ?? ""
        };
        Update(list.Assigner);
    }

    public void ResetKeys()
    {
        _lastKey = 0;
        _modifiers = 0;
        _lastSymbol = "";
    }

    private string Translate()
    {
        var modifiers = _modifiers;
        var sb = new StringBuilder();
        var first = true;
        if ((modifiers & KeyModifiers.Control) != 0)
            Append("Ctrl");
        if ((modifiers & KeyModifiers.Alt) != 0)
            Append("Alt");
        if ((modifiers & KeyModifiers.Shift) != 0)
            Append("Shift");
        if (!string.IsNullOrEmpty(_lastSymbol))
            Append(_lastSymbol);
        return sb.ToString();

        void Append(string s)
        {
            if (!first)
                sb.Append(" + ");
            first = false;
            sb.Append(s);
        }
    }

    private void Update(ShortcutAssigner assigner, bool finalize = false)
    {
        for (var i = 0; i < assigner.Active.Count; i++)
        {
            if (assigner.Active[i].InputMethodName != NullShortcut.InputMethodName)
                continue;
            var friendlyName = Translate();
            if (string.IsNullOrEmpty(friendlyName))
                assigner.Active.RemoveAt(i);
            else if (!finalize)
                assigner.Active[i] = NullShortcut with {FriendlyName = friendlyName};
            return;
        }

        assigner.Active.Add(NullShortcut with {FriendlyName = Translate()});
    }

}
#endif
