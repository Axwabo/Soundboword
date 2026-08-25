using Avalonia.Win32.Input;
using Soundboword.Inputs;

namespace Soundboword.Windows.GlobalHotkeys;

public sealed class GlobalHotkeyInput : IInputMethod
{

    public const string Name = "Global Hotkeys";

    private readonly HashSet<int> _detected = [];

    private readonly Dictionary<int, Gesture> _gestures = [];
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
            RegisterHotKey(id, gesture);
        }

        ShortcutList.ShortcutsChanged += ShortcutListOnShortcutsChanged;
    }

    public void Dispose() => Win32Properties.RemoveWndProcHookCallback(_topLevel, WndProc);

    private void RegisterHotKey(int id, Gesture gesture) => Loseterop.RegisterHotKey(_hWnd, id, (uint) gesture.Modifiers, (uint) KeyInterop.VirtualKeyFromKey(gesture.Key));

    private void ShortcutListOnShortcutsChanged()
    {
        var gestures = _repository.Gestures;
        foreach (var gesture in gestures)
        {
            var id = gesture.GetHashCode();
            if (!_gestures.TryAdd(id, gesture))
                continue;
            RegisterHotKey(id, gesture);
            _detected.Add(id);
        }

        foreach (var key in _gestures.Keys)
        {
            if (_detected.Contains(key))
        }

        _detected.UnionWith(_gestures.Keys);
        foreach (var gesture in gestures)
        {
            var id = gesture.GetHashCode();
            _detected.Remove(id);
            if (!_gestures.TryAdd(id, gesture))
                RegisterHotKey(id, gesture);
        }

        foreach (var i in _detected)
            if (_gestures.Remove(i))
                Loseterop.UnregisterHotKey(_hWnd, i);
        _detected.Clear();
    }

    private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg != 0x0312 || hWnd != _hWnd || !_gestures.TryGetValue(wParam.ToInt32(), out var gesture))
            return IntPtr.Zero;
        _list.Trigger(gesture, Name); // TODO: handled state
        return IntPtr.Zero;
    }

}
