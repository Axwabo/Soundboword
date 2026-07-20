using Soundboword.Inputs;

namespace Soundboword.Linux.Services;

public sealed class GlobalShortcutsInput : IInputMethod
{

    public const string Name = "XDG Global Shortcuts";

    public void Dispose()
    {
    }

}
