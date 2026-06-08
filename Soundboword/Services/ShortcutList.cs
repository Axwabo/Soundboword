using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Soundboword.Inputs;
using Soundboword.Models;
using Soundboword.ViewModels;

namespace Soundboword.Services;

public static class ShortcutList
{

    private static readonly Dictionary<SoundViewModel, HashSet<Shortcut>> ShortcutsBySound = [];

    public static IEnumerable<Shortcut> ForSound(SoundViewModel sound) => ShortcutsBySound.TryGetValue(sound, out var set) ? set : Enumerable.Empty<Shortcut>();

    public static int FindIndex(string methodName, SoundViewModel sound)
    {
        for (var i = 0; i < Shortcuts.Count; i++)
            if (Shortcuts[i].MethodName == methodName && Shortcuts[i].Sound == sound)
                return i;
        return -1;
    }

    public static void RemoveAll(SoundViewModel sound) => ShortcutsBySound.Remove(sound);

    public static void RemoveAll(string method)
    {
        for (var i = Shortcuts.Count - 1; i >= 0; i--)
            if (Shortcuts[i].MethodName == method)
                Shortcuts.RemoveAt(i);
    }

}
