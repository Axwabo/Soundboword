using Avalonia.Input;
using Soundboword.Inputs;

namespace Soundboword.Windows.GlobalHotkeys;

[RegisterSingleton<IShortcutRepository>(Duplicate = DuplicateStrategy.Append)]
public sealed class GlobalHotkeyRepository : ShortcutRepository<KeyGesture>
{

    public GlobalHotkeyRepository(UserData data, AudioManager audioManager, SoundList soundList) : base(data, audioManager, soundList, GlobalHotkeysInput.Name, gesture => gesture.ToString(), SourceGenerationContext.Default.DictionaryStringKeyGesture)
    {
    }

    // TODO: allow same gestures and use shortcut as id?
    public IEnumerable<KeyGesture> Gestures => Keys.Distinct();

}
