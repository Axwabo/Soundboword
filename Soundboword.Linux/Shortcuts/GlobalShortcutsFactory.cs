using Avalonia.Threading;
using Soundboword.Inputs;

namespace Soundboword.Linux.Shortcuts;

[RegisterSingleton<IInputFactory>(Duplicate = DuplicateStrategy.Append)]
public sealed class GlobalShortcutsFactory : IInputFactory
{

    private readonly GlobalShortcutsPortal _portal;

    private readonly ShortcutList _shortcuts;

    public GlobalShortcutsFactory(ShortcutList shortcuts, GlobalShortcutsPortal portal)
    {
        _shortcuts = shortcuts;
        _portal = portal;
        IsAvailable = _portal.IsAvailable;
    }

    public string Name => GlobalShortcutsInput.Name;

    public bool IsAvailable { get; }

    public async Task<IInputMethod?> ActivateAsync()
    {
        if (!_portal.IsAvailable)
            return null;
        var disposable = await _portal.WatchActivatedAsync(s => Dispatcher.UIThread.Post(() => _shortcuts.Trigger(s, GlobalShortcutsInput.Name)));
        return disposable == null ? null : new GlobalShortcutsInput(disposable, _portal, _shortcuts);
    }

}
