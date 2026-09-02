using Avalonia.Win32.Input;
using Soundboword.Inputs;

namespace Soundboword.Windows.GlobalHotkeys;

public sealed class GlobalHotkeyInput : IInputMethod
{

    public const string Name = "Global Hotkeys";

    private readonly Dictionary<int, Gesture> _gestures = [];
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
        RegisterAll();
        _list.Assigner.PropertyChanged += AssignerOnPropertyChanged;
        ShortcutList.ShortcutsChanged += ShortcutListOnShortcutsChanged;
        Win32Properties.AddWndProcHookCallback(topLevel, WndProc);
    }

    public void Dispose()
    {
        _list.Assigner.PropertyChanged -= AssignerOnPropertyChanged;
        ShortcutList.ShortcutsChanged -= ShortcutListOnShortcutsChanged;
        Win32Properties.RemoveWndProcHookCallback(_topLevel, WndProc);
    }

    private void RegisterAll()
    {
        foreach (var gesture in _repository.Gestures)
        {
            var id = gesture.GetHashCode();
            _gestures[id] = gesture;
            RegisterHotKey(id, gesture);
        }
    }

    private void RegisterHotKey(int id, Gesture gesture)
        => Loseterop.RegisterHotKey(_hWnd, id, (uint) gesture.Modifiers, (uint) KeyInterop.VirtualKeyFromKey(gesture.Key));

    private void ShortcutListOnShortcutsChanged()
    {
        foreach (var key in _gestures.Keys)
            _toRemove.Add(key);
        foreach (var gesture in _repository.Gestures)
        {
            var id = gesture.GetHashCode();
            _toRemove.Remove(id);
            if (_gestures.TryAdd(id, gesture))
                RegisterHotKey(id, gesture);
        }

        foreach (var i in _toRemove)
            if (_gestures.Remove(i))
                Loseterop.UnregisterHotKey(_hWnd, i);
        _toRemove.Clear();
    }

    private void AssignerOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(ShortcutAssigner.IsAssigning))
            return;
        if (!_list.Assigner.IsAssigning)
        {
            RegisterAll();
            return;
        }

        foreach (var key in _gestures.Keys)
            Loseterop.UnregisterHotKey(_hWnd, key);
        _gestures.Clear();
    }

    private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg != 0x0312 || hWnd != _hWnd || !_gestures.TryGetValue(wParam.ToInt32(), out var gesture))
            return IntPtr.Zero;
        _list.Trigger(gesture, Name); // TODO: handled state
        return IntPtr.Zero;
    }

}
