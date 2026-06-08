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

    public ShortcutList(SoundList sounds, params IEnumerable<IShortcutRepository> repositories)
    {
        _sounds = sounds;
        _repositories = repositories.ToList();
        foreach (var sound in sounds.Sounds)
            _all.UnionWith(ForSound(sound));
    }

    public IEnumerable<Shortcut> ForSound(SoundViewModel sound)
    {
        foreach (var repository in _repositories)
        foreach (var shortcut in repository.GetAll(sound))
            yield return shortcut;
    }

    public void Trigger<T>(T key) where T : notnull
    {
        if (_sounds.Editor.Listening is not { } sound)
        {
            foreach (var repository in _repositories)
                if (repository is ShortcutRepository<T> implementation)
                    implementation.Trigger(key);
            return;
        }

        var changed = false;
        foreach (var repository in _repositories)
            if (repository is ShortcutRepository<T> implementation)
                changed |= implementation.Assign(key, sound, _all);
        _sounds.Editor.CancelShortcutAddition();
        if (changed)
            ShortcutsChanged?.Invoke();
    }

    public void Remove(SoundViewModel sound)
    {
        if (_sounds.Editor.Listening == sound)
            _sounds.Editor.CancelShortcutAddition();
        foreach (var repository in _repositories)
            repository.RemoveAll(sound);
        _all.RemoveWhere(e => e.Sound == sound);
    }

    public void CommitAll()
    {
        foreach (var repository in _repositories)
            repository.Commit();
    }

}
