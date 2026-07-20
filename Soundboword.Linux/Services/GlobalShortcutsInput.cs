using Soundboword.Inputs;

namespace Soundboword.Linux.Services;

public sealed class GlobalShortcutsInput : IInputMethod
{

    public const string Name = "XDG Global Shortcuts";

    private readonly IDisposable _disposable;

    public GlobalShortcutsInput(IDisposable disposable) => _disposable = disposable;

    public void Dispose() => _disposable.Dispose();

}
