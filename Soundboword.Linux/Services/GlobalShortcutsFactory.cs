using Soundboword.Inputs;

namespace Soundboword.Linux.Services;

[RegisterSingleton<IInputFactory>(Duplicate = DuplicateStrategy.Append)]
public sealed class GlobalShortcutsFactory : IInputFactory
{

    public string Name => "XDG Global Shortcuts";

    public bool IsAvailable => true;

    public IInputMethod? Activate() => null;

}
