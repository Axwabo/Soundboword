using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.ApplicationLifetimes;
using Soundboword.Inputs;
using Soundboword.Models;
using Soundboword.ViewModels;

namespace Soundboword.Services;

public sealed class ShortcutList
{

    private readonly HashSet<Shortcut> _all = [];

    private readonly List<IShortcutRepository> _repositories;

    public ShortcutList(IClassicDesktopStyleApplicationLifetime? lifetime, ShortcutAssigner assigner, params IEnumerable<IShortcutRepository> repositories)
    {
        Assigner = assigner;
        _repositories = repositories.ToList();
        foreach (var repository in _repositories)
            _all.UnionWith(repository.All);
        lifetime?.Exit += (_, _) =>
        {
            foreach (var repository in _repositories)
                repository.Commit();
        };
    }

    public ShortcutAssigner Assigner { get; }

    public event Action? ShortcutsChanged;

    public IEnumerable<Shortcut> ForSound(SoundViewModel sound)
    {
        foreach (var repository in _repositories)
        foreach (var shortcut in repository.GetAll(new TriggerSoundAction(sound)))
            yield return shortcut;
    }

    public Shortcut? ForStopAll(string inputMethod)
    {
        foreach (var repository in _repositories)
            if (repository.InputMethodName == inputMethod)
                return repository.GetAll(StopAllSoundsAction.Instance).FirstOrDefault();
        return null;
    }

    public void Trigger<T>(T key, string inputMethod) where T : notnull
    {
        if (!Assigner.IsAssigning)
        {
            foreach (var repository in _repositories)
                if (repository is ShortcutRepository<T> implementation)
                    implementation.Trigger(key);
            return;
        }

        if (Assigner.InputMethodFilter is { } filter && filter != inputMethod)
            return;
        Assigner.IsAssigning = false;
        if (Assigner.Target is not { } action)
            return;
        var changed = false;
        foreach (var repository in _repositories)
            if (repository is ShortcutRepository<T> implementation)
                changed |= implementation.Assign(key, action, _all);
        if (changed)
            ShortcutsChanged?.Invoke();
    }

    public void Remove(ShortcutAction action)
    {
        if (Assigner.Target == action)
            Assigner.IsAssigning = false;
        foreach (var repository in _repositories)
            repository.RemoveAll(action);
        var removed = _all.RemoveWhere(e => e.Action == action);
        if (removed != 0)
            ShortcutsChanged?.Invoke();
    }

}
