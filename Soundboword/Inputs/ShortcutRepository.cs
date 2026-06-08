using System;
using System.Collections.Generic;
using System.IO;
using Soundboword.Models;
using Soundboword.Services;
using Soundboword.ViewModels;

namespace Soundboword.Inputs;

public abstract class ShortcutRepository<T> : IShortcutRepository where T : notnull
{

    public string InputMethodName { get; }

    private readonly Func<T, string> _toFriendlyName;

    private readonly Dictionary<Guid, T> _map;
    private readonly Dictionary<T, HashSet<Shortcut>> _shortcuts = [];

    protected ShortcutRepository(SoundList soundList, string inputMethodName, Func<T, string> toFriendlyName)
    {
        InputMethodName = inputMethodName;
        _toFriendlyName = toFriendlyName;
        _map = UserData.Load(File, () => new Dictionary<Guid, T>());
        foreach (var sound in soundList.Sounds)
            if (_map.TryGetValue(sound.Id, out var key))
                Assign(key, sound, null);
    }

    private string File => Path.Combine(UserData.Folder, $"{InputMethodName}.json");

    public bool Assign(T key, SoundViewModel sound, HashSet<Shortcut>? all)
    {
        if (all != null && _map.TryGetValue(sound.Id, out var assigned))
        {
            if (EqualityComparer<T>.Default.Equals(key, assigned))
                return false;
            Unbind(assigned, sound, all);
        }

        if (!_shortcuts.TryGetValue(key, out var set))
            _shortcuts[key] = set = [];
        _map[sound.Id] = key;
        var shortcut = new Shortcut(InputMethodName, _toFriendlyName(key), sound);
        all?.Add(shortcut);
        return set.Add(shortcut);
    }

    public void Trigger(T key)
    {
        if (!_shortcuts.TryGetValue(key, out var list))
            return;
        foreach (var shortcut in list)
            AudioManager.Trigger(shortcut.Sound);
    }

    private void Unbind(T assigned, SoundViewModel sound, HashSet<Shortcut> all)
    {
        if (!_shortcuts.TryGetValue(assigned, out var set))
            return;
        foreach (var shortcut in set)
        {
            if (shortcut.Sound != sound)
                continue;
            set.Remove(shortcut);
            all.Remove(shortcut);
            break;
        }
    }

    public IEnumerable<Shortcut> GetAll(SoundViewModel sound)
    {
        foreach (var set in _shortcuts.Values)
        foreach (var shortcut in set)
            if (shortcut.Sound == sound)
                yield return shortcut;
    }

    public void RemoveAll(SoundViewModel sound)
    {
        foreach (var set in _shortcuts.Values)
            set.RemoveWhere(e => e.Sound == sound);
        _map.Remove(sound.Id);
    }

    public void Commit() => UserData.Save(File, _map);

}
