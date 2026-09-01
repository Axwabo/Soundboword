using Soundboword.Inputs;

namespace Soundboword.Windows.GlobalHotkeys;

[RegisterSingleton<IInputFactory>(Duplicate = DuplicateStrategy.Append)]
public sealed class GlobalHotkeyFactory : IInputFactory
{

    private readonly ILoggerFactory _factory;

    private readonly ShortcutList _list;

    private readonly TopLevel _topLevel;

    public GlobalHotkeyFactory(TopLevel topLevel, ShortcutList list, ILoggerFactory factory)
    {
        _topLevel = topLevel;
        _list = list;
        _factory = factory;
    }

    public string Name => GlobalHotkeyInput.Name;

    public bool IsAvailable => true;

    public Task<IInputMethod?> ActivateAsync() => Task.FromResult<IInputMethod?>(new GlobalHotkeyInput(_topLevel, _list, _factory));

}
