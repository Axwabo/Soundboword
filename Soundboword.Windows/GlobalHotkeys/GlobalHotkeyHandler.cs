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

    private readonly ShortcutAssigner _assigner;

    private readonly ShortcutList _list;

    private Key _lastKey;

    private string _lastSymbol = "";

    private KeyModifiers _modifiers;

    // TODO: circular dependency
    public GlobalHotkeyHandler(ShortcutList list)
    {
        _list = list;
        _assigner = list.Assigner;
    }

    public void OnPressed(KeyEventArgs eventArgs, ShortcutAssigner assigner)
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
            // TODO: localization, cuz apparently this isn't localized :sob:
            _ => eventArgs.KeySymbol?.ToUpper()
        };
        if (!string.IsNullOrEmpty(symbol))
            _lastSymbol = symbol;
        Update();
    }

    public void OnReleased(KeyEventArgs eventArgs, ShortcutAssigner assigner)
    {
        var modifier = GetModifier(eventArgs.Key);
        if (modifier != KeyModifiers.None)
        {
            _modifiers &= ~modifier;
            Update();
        }
        else if (_lastKey == eventArgs.Key)
            Update(true);
        else
            Update();
    }

    public void OnTextInput(TextInputEventArgs eventArgs, ShortcutAssigner assigner)
    {
        _lastSymbol = _lastKey == Key.Space ? "Space" : eventArgs.Text?.ToUpper() ?? "";
        Update();
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

    private void Update(bool finalize = false)
    {
        for (var i = 0; i < _assigner.Active.Count; i++)
        {
            if (_assigner.Active[i].InputMethodName != NullShortcut.InputMethodName)
                continue;
            var friendlyName = Translate();
            if (string.IsNullOrEmpty(friendlyName))
                _assigner.Active.RemoveAt(i);
            else if (!finalize)
                _assigner.Active[i] = NullShortcut with {FriendlyName = friendlyName, IsEphemeral = true};
            else
                _list.Trigger(new KeyGesture(_lastKey, _modifiers), GlobalHotkeyInput.Name);
            return;
        }

        // TODO: check finalize should probably never happen here
        _assigner.Active.Add(NullShortcut with {FriendlyName = Translate(), IsEphemeral = !finalize});
    }

}
