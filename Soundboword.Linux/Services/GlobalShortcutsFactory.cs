using Soundboword.Inputs;

namespace Soundboword.Linux.Services;

[RegisterSingleton<IInputFactory>(Duplicate = DuplicateStrategy.Append)]
public sealed class GlobalShortcutsFactory : IInputFactory
{

    private readonly GlobalShortcutsPortal _portal;

    public GlobalShortcutsFactory(GlobalShortcutsPortal portal)
    {
        _portal = portal;
        IsAvailable = _portal.IsAvailable;
    }

    public string Name => GlobalShortcutsInput.Name;

    public bool IsAvailable { get; }

    public async Task<IInputMethod?> ActivateAsync()
    {
        await Task.Delay(1000);
        return null;
    }

}
