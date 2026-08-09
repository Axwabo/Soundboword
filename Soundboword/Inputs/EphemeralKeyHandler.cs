using System.Text;
using Avalonia.Input;

namespace Soundboword.Inputs;

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

    private readonly ShortcutAssigner _assigner;

    private string _lastSymbol = "";

    private KeyModifiers _modifiers;

    public EphemeralKeyHandler(ShortcutAssigner assigner) => _assigner = assigner;

    public void OnPressed(KeyEventArgs eventArgs)
    {
        var modifier = GetModifier(eventArgs.Key);
        if (modifier != KeyModifiers.None)
            _modifiers |= modifier;
        var symbol = eventArgs.Key switch
        {
            Key.Up => "Up Arrow",
            Key.Down => "Down Arrow",
            Key.Left => "Left Arrow",
            Key.Right => "Right Arrow",
            // TODO: localization, cuz apparently this isn't localized :sob:
            _ => eventArgs.KeySymbol
        };
        if (!string.IsNullOrEmpty(symbol))
            _lastSymbol = symbol;
        Update();
    }

    public void OnReleased(KeyEventArgs eventArgs)
    {
        var modifier = GetModifier(eventArgs.Key);
        if (modifier != KeyModifiers.None)
            _modifiers &= ~modifier;
        else
            _lastSymbol = "";
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

    private void Update()
    {
        for (var i = 0; i < _assigner.Active.Count; i++)
        {
            if (!_assigner.Active[i].IsEphemeral)
                continue;
            _assigner.Active[i] = NullShortcut with {FriendlyName = Translate()};
            return;
        }

        _assigner.Active.Add(NullShortcut with {FriendlyName = Translate()});
    }

}
