using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Soundboword.Models;
using Soundboword.ViewModels;

namespace Soundboword.Services;

public sealed class ShortcutList
{

    public static Dictionary<Guid, T> Load<T>(string name, Func<T, string> friendlyNameConverter)
    {
        var saved = UserData.Load(Path.Combine(UserData.Folder, name + ".json"), () => new Dictionary<Guid, T>());
        foreach (var (key, value) in saved)
        {
            ShortcutsCache<> 
        }
    }

    private readonly Dictionary<SoundViewModel, HashSet<Shortcut>> ShortcutsBySound = [];

    public IEnumerable<Shortcut> ForSound(SoundViewModel sound) => ShortcutsBySound.TryGetValue(sound, out var set) ? set : Enumerable.Empty<Shortcut>();

    public void RemoveAll(SoundViewModel sound) => ShortcutsBySound.Remove(sound);

}

file static class ShortcutsCache<T>
{

    private static readonly Dictionary<Guid, T> ById = [];

}
