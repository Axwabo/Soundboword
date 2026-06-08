using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Soundboword.Inputs;
using Soundboword.Models;
using Soundboword.ViewModels;

namespace Soundboword.Services;

public sealed class ShortcutList
{

    private readonly SoundEditingContext _editingContext;

    private readonly List<IShortcutRepository> _repositories;

    public ObservableCollection<Shortcut> All { get; } = [];

    public ShortcutList(SoundEditingContext editingContext, params IEnumerable<IShortcutRepository> repositories)
    {
        _editingContext = editingContext;
        _repositories = repositories.ToList();
    }

    public IEnumerable<Shortcut> ForSound(SoundViewModel sound)
    {
        foreach (var repository in _repositories)
        foreach (var shortcut in repository.GetAll(sound))
            yield return shortcut;
    }

    public void Trigger<T>(T key) where T : notnull
    {
        if (_editingContext.Listening is not { } sound)
        {
            foreach (var repository in _repositories)
                if (repository is ShortcutRepository<T> implementation)
                    implementation.Trigger(key);
            return;
        }

        foreach (var repository in _repositories)
            if (repository is ShortcutRepository<T> implementation && implementation.Assign(key, sound) is { } shortcut)
                All.Add(shortcut);
        _editingContext.CancelShortcutAddition();
    }

    public void Remove(SoundViewModel sound)
    {
        if (_editingContext.Listening == sound)
            _editingContext.CancelShortcutAddition();
        foreach (var repository in _repositories)
            repository.RemoveAll(sound);
        for (var i = All.Count - 1; i >= 0; i--)
            if (All[i].Sound == sound)
                All.RemoveAt(i);
    }

}
