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

    private readonly HashSet<int> _toRemove = [];

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
            RegisterHotKey(id, gesture);
        }

        ShortcutList.ShortcutsChanged += ShortcutListOnShortcutsChanged;
    }

    public void Dispose() => Win32Properties.RemoveWndProcHookCallback(_topLevel, WndProc);

    private void RegisterHotKey(int id, KeyGesture gesture) => Loseterop.RegisterHotKey(_hWnd, id, (uint) gesture.KeyModifiers, (uint) KeyInterop.VirtualKeyFromKey(gesture.Key));

    private void ShortcutListOnShortcutsChanged()
    {
        _toRemove.UnionWith(_gestures.Keys);
        foreach (var gesture in _repository.Gestures)
        {
            var id = gesture.GetHashCode();
            _toRemove.Remove(id);
            if (!_gestures.TryAdd(id, gesture))
                RegisterHotKey(id, gesture);
        }

        foreach (var i in _toRemove)
            if (_gestures.Remove(i))
                Loseterop.UnregisterHotKey(_hWnd, i);
        _toRemove.Clear();
    }

    private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg != 0x0312 || hWnd != _hWnd || !_gestures.TryGetValue(wParam.ToInt32(), out var gesture))
            return IntPtr.Zero;
        _list.Trigger(gesture, Name); // TODO: handled state
        return IntPtr.Zero;
    }

}
