using System.Text;
using Avalonia.Input;

namespace Soundboword.Inputs;

[RegisterSingleton<IAssignmentKeyHandler>]
public sealed class EphemeralKeyHandler : IAssignmentKeyHandler
{

    private static readonly Shortcut NullShortcut = new("Dummy", "", null!, true);

    private static string Translate(KeyEventArgs eventArgs)
    {
        var sb = new StringBuilder();
        var first = true;
        if ((eventArgs.KeyModifiers & KeyModifiers.Control) != 0 || eventArgs.Key is Key.LeftCtrl or Key.RightCtrl)
            Append("Ctrl");
        if ((eventArgs.KeyModifiers & KeyModifiers.Alt) != 0 || eventArgs.Key is Key.LeftAlt or Key.RightAlt)
            Append("Alt");
        if ((eventArgs.KeyModifiers & KeyModifiers.Shift) != 0 || eventArgs.Key is Key.LeftShift or Key.RightShift)
            Append("Shift");
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
            Append(symbol);
        return sb.ToString();

        void Append(string s)
        {
            if (!first)
                sb.Append(" + ");
            first = false;
            sb.Append(s);
        }
    }

    private readonly ShortcutAssigner _assigner;

    public EphemeralKeyHandler(ShortcutAssigner assigner) => _assigner = assigner;

    public void OnPressed(KeyEventArgs eventArgs)
    {
        for (var i = 0; i < _assigner.Active.Count; i++)
        {
            if (!_assigner.Active[i].IsEphemeral)
                continue;
            _assigner.Active[i] = NullShortcut with {FriendlyName = Translate(eventArgs)};
            return;
        }

        _assigner.Active.Add(NullShortcut with {FriendlyName = Translate(eventArgs)});
    }

    public void OnReleased(KeyEventArgs eventArgs)
    {
    }

}
