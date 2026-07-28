using Soundboword.Logging;

namespace Soundboword.Settings.General;

[RegisterSingleton<ISettingsProvider>(Duplicate = DuplicateStrategy.Append)]
public sealed class PreferencesProvider : ISettingsProvider
{

    public PreferencesProvider(Preferences preferences) => Sections = [preferences, LogPreferences.Instance];

    public IEnumerable<SettingsSection> Sections { get; }

}
