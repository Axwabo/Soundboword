using System.Collections.ObjectModel;
using Soundboword.Models;
using Soundboword.ViewModels;

namespace Soundboword.Services;

public static class ShortcutList
{

    public static ObservableCollection<IShortcut> Shortcuts { get; } = [];

    public static int FindIndex(string methodName, SoundViewModel sound)
    {
        for (var i = 0; i < Shortcuts.Count; i++)
            if (Shortcuts[i].MethodName == methodName && Shortcuts[i].Sound == sound)
                return i;
        return -1;
    }

    public static void RemoveAll(SoundViewModel sound)
    {
        for (var i = Shortcuts.Count - 1; i >= 0; i--)
            if (Shortcuts[i].Sound == sound)
                Shortcuts.RemoveAt(i);
    }

}
