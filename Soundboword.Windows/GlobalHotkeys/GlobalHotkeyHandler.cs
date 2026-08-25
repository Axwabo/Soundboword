using System.Text;
using Avalonia.Input;
using Soundboword.Inputs;

namespace Soundboword.Windows.GlobalHotkeys;

[RegisterSingleton<IAssignmentKeyHandler>]
public sealed class GlobalHotkeyHandler : IAssignmentKeyHandler
{

    private static readonly Shortcut NullShortcut = new(GlobalHotkeyInput.Name, "", null!, true);

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
        Update(list);
    }

    public void OnReleased(KeyEventArgs eventArgs, ShortcutList list)
    {
        var modifier = GetModifier(eventArgs.Key);
        if (modifier != KeyModifiers.None)
        {
            _modifiers &= ~modifier;
            Update(list);
        }
        else if (_lastKey == eventArgs.Key)
            Update(list, true);
        else
            Update(list);
    }

    public void OnTextInput(TextInputEventArgs eventArgs, ShortcutList list)
    {
        _lastSymbol = _lastKey switch
        {
            Key.Space => "Space",
            Key.Enter => "Enter",
            _ => eventArgs.Text?.ToUpper() ?? ""
        };
        Update(list);
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

    private void Update(ShortcutList list, bool finalize = false)
    {
        var assigner = list.Assigner;
        for (var i = 0; i < assigner.Active.Count; i++)
        {
            if (assigner.Active[i].InputMethodName != NullShortcut.InputMethodName)
                continue;
            var friendlyName = Translate();
            if (string.IsNullOrEmpty(friendlyName))
                assigner.Active.RemoveAt(i);
            else if (!finalize)
                assigner.Active[i] = NullShortcut with {FriendlyName = friendlyName, IsEphemeral = true};
            else
                list.Trigger(new KeyGesture(_lastKey, _modifiers), GlobalHotkeyInput.Name);
            return;
        }

        // TODO: check finalize should probably never happen here
        assigner.Active.Add(NullShortcut with {FriendlyName = Translate(), IsEphemeral = !finalize});
    }

}
