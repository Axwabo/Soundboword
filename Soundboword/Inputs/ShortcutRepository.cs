using System;
using System.Collections.Generic;
using System.IO;
using Soundboword.Models;
using Soundboword.Services;

namespace Soundboword.Inputs;

public abstract class ShortcutRepository<T> : IShortcutRepository where T : notnull
{

    private readonly AudioManager _audioManager;
    private readonly string _inputMethodName;
    private readonly Func<T, string> _toFriendlyName;

    private readonly Dictionary<string, T> _map;
    private readonly Dictionary<T, HashSet<Shortcut>> _shortcuts = [];

    protected ShortcutRepository(AudioManager audioManager, SoundList soundList, string inputMethodName, Func<T, string> toFriendlyName)
    {
        _audioManager = audioManager;
        _inputMethodName = inputMethodName;
        _toFriendlyName = toFriendlyName;
        _map = UserData.Load(File, () => new Dictionary<string, T>());
        foreach (var sound in soundList.Sounds)
            if (_map.TryGetValue(sound.Id, out var key))
                Assign(key, new TriggerSoundAction(sound), null);
    }

    private string File => Path.Combine(UserData.Folder, $"{_inputMethodName}.json");

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
        var shortcut = new Shortcut(_inputMethodName, _toFriendlyName(key), action);
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

    public void Commit() => UserData.Save(File, _map);

}
