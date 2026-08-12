using Soundboword.Inputs;

namespace Soundboword.Windows.GlobalHotkeys;

[RegisterSingleton<IInputFactory>]
public sealed class GlobalHotkeyFactory : IInputFactory
{

    private readonly ShortcutList _list;

    private readonly TopLevel _topLevel;

    public GlobalHotkeyFactory(TopLevel topLevel, ShortcutList list)
    {
        _topLevel = topLevel;
        _list = list;
    }

    public string Name => "Global Hotkeys";

    public bool IsAvailable => true;

    public Task<IInputMethod?> ActivateAsync() => Task.FromResult<IInputMethod?>(new GlobalHotkeysInput(_topLevel, _list));

}
