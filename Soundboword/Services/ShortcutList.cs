using System;
using System.Collections.Generic;
using System.Linq;
using Soundboword.Inputs;
using Soundboword.Models;
using Soundboword.ViewModels;

namespace Soundboword.Services;

public sealed class ShortcutList
{

    private readonly SoundList _sounds;

    private readonly List<IShortcutRepository> _repositories;

    private readonly HashSet<Shortcut> _all = [];

    public event Action? ShortcutsChanged;

    public ShortcutList(Host? host, SoundList sounds, params IEnumerable<IShortcutRepository> repositories)
    {
        _sounds = sounds;
        _repositories = repositories.ToList();
        foreach (var sound in sounds.Sounds)
            _all.UnionWith(ForSound(sound));
        host?.Lifetime.Exit += (_, _) =>
        {
            foreach (var repository in _repositories)
                repository.Commit();
        };
    }

    public IEnumerable<Shortcut> ForSound(SoundViewModel sound)
    {
        foreach (var repository in _repositories)
        foreach (var shortcut in repository.GetAll(sound))
            yield return shortcut;
    }

    public void Trigger<T>(T key) where T : notnull
    {
        if (!_sounds.Editor.IsListeningForShortcuts)
        {
            foreach (var repository in _repositories)
                if (repository is ShortcutRepository<T> implementation)
                    implementation.Trigger(key);
            return;
        }

        var changed = false;
        foreach (var repository in _repositories)
            if (repository is ShortcutRepository<T> implementation)
                changed |= implementation.Assign(key, _sounds.Editor.Model, _all);
        _sounds.Editor.CancelShortcutAddition();
        if (changed)
            ShortcutsChanged?.Invoke();
    }

    public void Remove(SoundViewModel sound)
    {
        if (_sounds.Editor.IsListeningForShortcuts && _sounds.Editor.Model == sound)
            _sounds.Editor.CancelShortcutAddition();
        foreach (var repository in _repositories)
            repository.RemoveAll(sound);
        var removed = _all.RemoveWhere(e => e.Sound == sound);
        if (removed != 0)
            ShortcutsChanged?.Invoke();
    }

}
