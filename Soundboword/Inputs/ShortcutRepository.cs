using System.Text.Json.Serialization.Metadata;

namespace Soundboword.Inputs;

public abstract class ShortcutRepository<T> : IShortcutRepository where T : notnull
{

    private readonly AudioManager _audioManager;
    private readonly Dictionary<string, T> _map;
    private readonly Dictionary<T, HashSet<Shortcut>> _shortcuts = [];
    private readonly Func<T, string> _toFriendlyName;
    private readonly JsonTypeInfo<Dictionary<string, T>>? _typeInfo;

    protected ShortcutRepository(AudioManager audioManager, SoundList soundList, string inputMethodName, Func<T, string> toFriendlyName, JsonTypeInfo<Dictionary<string, T>>? typeInfo)
    {
        InputMethodName = inputMethodName;
        _audioManager = audioManager;
        _toFriendlyName = toFriendlyName;
        _typeInfo = typeInfo;
        _map = UserData.Load(InputMethodName, () => new Dictionary<string, T>(), typeInfo);
        InitializeMap(_map, soundList);
    }

    public virtual ShortcutList? List { get; set; }

    public string InputMethodName { get; }

    public IEnumerable<Shortcut> All => _shortcuts.SelectMany(e => e.Value);

    public IEnumerable<Shortcut> GetAll(ShortcutAction action)
    {
        foreach (var set in _shortcuts.Values)
        foreach (var shortcut in set)
            if (shortcut.Action == action)
                yield return shortcut;
    }

    public void RemoveAll(ShortcutAction action)
    {
        foreach (var set in _shortcuts.Values)
            set.RemoveWhere(e => e.Action == action);
        _map.Remove(action.Id);
    }

    public void Commit() => UserData.Save(InputMethodName, _map, _typeInfo);

    protected void InitializeMap(Dictionary<string, T> dictionary, SoundList soundList)
    {
        foreach (var sound in soundList.Sounds)
            if (dictionary.TryGetValue(sound.Id, out var key))
                Assign(key, new TriggerSoundAction(sound), null);
        if (dictionary.TryGetValue(StopAllSoundsAction.Instance.Id, out var stopAllKey))
            Assign(stopAllKey, StopAllSoundsAction.Instance, null);
    }

    public bool Assign(T key, ShortcutAction action, HashSet<Shortcut>? all)
    {
        if (all != null && _map.TryGetValue(action.Id, out var assigned))
        {
            if (EqualityComparer<T>.Default.Equals(key, assigned))
                return false;
            Unbind(assigned, action, all);
        }

        if (!_shortcuts.TryGetValue(key, out var set))
            _shortcuts[key] = set = [];
        _map[action.Id] = key;
        var shortcut = new Shortcut(InputMethodName, _toFriendlyName(key), action);
        all?.Add(shortcut);
        return set.Add(shortcut);
    }

    public void Trigger(T key)
    {
        if (!_shortcuts.TryGetValue(key, out var list))
            return;
        foreach (var shortcut in list)
            shortcut.Trigger(_audioManager);
    }

    private void Unbind(T assigned, ShortcutAction action, HashSet<Shortcut> all)
    {
        if (!_shortcuts.TryGetValue(assigned, out var set))
            return;
        foreach (var shortcut in set)
        {
            if (shortcut.Action != action)
                continue;
            set.Remove(shortcut);
            all.Remove(shortcut);
            break;
        }
    }

}
