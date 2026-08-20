using Avalonia.Input;
using Avalonia.Win32.Input;
using Soundboword.Inputs;

namespace Soundboword.Windows.GlobalHotkeys;

public sealed class GlobalHotkeyInput : IInputMethod
{

    public const string Name = "Global Hotkeys";

    private readonly Dictionary<int, KeyGesture> _gestures = [];
    private readonly IntPtr _hWnd;
    private readonly ShortcutList _list;
    private readonly GlobalHotkeyRepository _repository;

    private readonly TopLevel _topLevel;

    public GlobalHotkeyInput(TopLevel topLevel, ShortcutList list)
    {
        _topLevel = topLevel;
        _hWnd = topLevel.TryGetPlatformHandle()!.Handle;
        _list = list;
        _repository = list.RequireRepository<GlobalHotkeyRepository>();
        Win32Properties.AddWndProcHookCallback(topLevel, WndProc);
        foreach (var gesture in _repository.Gestures)
        {
            var id = gesture.GetHashCode();
            _gestures[id] = gesture;
            Loseterop.RegisterHotKey(_hWnd, id, (uint) gesture.KeyModifiers, (uint) KeyInterop.VirtualKeyFromKey(gesture.Key));
        }

        ShortcutList.ShortcutsChanged += ShortcutListOnShortcutsChanged;
    }

    public void Dispose() => Win32Properties.RemoveWndProcHookCallback(_topLevel, WndProc);

    private void ShortcutListOnShortcutsChanged()
    {
        var skip = _repository.Gestures.ToHashSet();
        foreach (var (key, value) in _gestures.ToList())
        {
            if (skip.Contains(value))
                continue;
            Loseterop.UnregisterHotKey(_hWnd, key);
            _gestures.Remove(key);
        }
    }

    private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg != 0x0312 || hWnd != _hWnd || !_gestures.TryGetValue(wParam.ToInt32(), out var gesture))
            return IntPtr.Zero;
        _list.Trigger(gesture, Name); // TODO: handled state
        return IntPtr.Zero;
    }

}
