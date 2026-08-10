using Avalonia.Input;
using Soundboword.Inputs;

namespace Soundboword.Windows.GlobalHotkeys;

public sealed class GlobalHotkeyRepository : ShortcutRepository<KeyGesture>
{

    public GlobalHotkeyRepository(UserData data, AudioManager audioManager, SoundList soundList) : base(data, audioManager, soundList, "Global Hotkeys", gesture => gesture.ToString(), SourceGenerationContext.Default.DictionaryStringKeyGesture)
    {
    }

}
