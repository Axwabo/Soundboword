using Avalonia.Input;
using Soundboword.Inputs;

namespace Soundboword.Windows.GlobalHotkeys;

[RegisterSingleton<IShortcutRepository>(Duplicate = DuplicateStrategy.Append)]
public sealed class GlobalHotkeyRepository : ShortcutRepository<KeyGesture>
{

    public GlobalHotkeyRepository(UserData data, AudioManager audioManager, SoundList soundList) : base(
        data,
        audioManager,
        soundList,
        GlobalHotkeyInput.Name,
        gesture => gesture.ToString(),
        SourceGenerationContext.Default.DictionaryStringKeyGesture
    )
    {
    }

    public HashSet<KeyGesture> Gestures
    {
        get
        {
            field.Clear();
            field.EnsureCapacity(Keys.Count);
            foreach (var gesture in Keys)
                field.Add(gesture);
            return field;
        }
    } = [];

}
