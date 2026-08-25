using System.Text;
using Avalonia.Input;
using Soundboword.Inputs;

namespace Soundboword.Windows.GlobalHotkeys;

[RegisterSingleton<IAssignmentKeyHandler>]
public sealed class GlobalHotkeyHandler : IAssignmentKeyHandler
{

    private static readonly Shortcut NullShortcut = new(GlobalHotkeyInput.Name, "", null!, true);

    private Key _lastKey;

    private string _lastSymbol = "";

    private KeyModifiers _modifiers;

    public void OnPressed(KeyEventArgs eventArgs, ShortcutList list)
    {
        var modifier = eventArgs.Key.GetModifier();
        if (modifier != KeyModifiers.None)
            _modifiers |= modifier;
        else
            _lastKey = eventArgs.Key;
        var symbol = eventArgs.Translate();
        if (!string.IsNullOrEmpty(symbol))
            _lastSymbol = symbol;
        Update(list);
    }

    public void OnReleased(KeyEventArgs eventArgs, ShortcutList list)
    {
        var modifier = eventArgs.Key.GetModifier();
        if (modifier == KeyModifiers.None)
        {
            Update(list, _lastKey == eventArgs.Key);
            return;
        }

        _modifiers &= ~modifier;
        Update(list);
    }

    public void OnTextInput(TextInputEventArgs eventArgs, ShortcutList list)
    {
        if (!_lastKey.HasTranslationOverride)
            _lastSymbol = eventArgs.Text?.ToUpper() ?? "";
        Update(list);
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
            else if (finalize)
                list.Trigger(new Gesture(_lastKey, _modifiers, friendlyName), GlobalHotkeyInput.Name);
            else
                assigner.Active[i] = NullShortcut with {FriendlyName = friendlyName};
            return;
        }

        assigner.Active.Add(NullShortcut with {FriendlyName = Translate()});
    }

}
