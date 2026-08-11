using Soundboword.Inputs;

namespace Soundboword.Windows.GlobalHotkeys;

public sealed class GlobalHotkeysInput : IInputMethod
{

    public const string Name = "Global Hotkeys";

    private readonly Dictionary<int, Shortcut> _ids = [];
    private readonly ShortcutList _list;

    private readonly TopLevel _topLevel;

    public GlobalHotkeysInput(TopLevel topLevel, ShortcutList list)
    {
        _topLevel = topLevel;
        _list = list;
        Win32Properties.AddWndProcHookCallback(topLevel, WndProc);
        // TODO: get repo
        foreach (var shortcut in list.ForRepository(Name))
        {
            Loseterop.RegisterHotKey()
        }
    }

    public void Dispose() => Win32Properties.RemoveWndProcHookCallback(_topLevel, WndProc);

    private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg != 0x0312)
            return IntPtr.Zero;
    }

}
