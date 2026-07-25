namespace Soundboword.Settings.General;

[RegisterSingleton<ISettingsProvider>]
public sealed class PreferencesProvider : ISettingsProvider
{

    public PreferencesProvider(Preferences preferences) => Sections = [preferences];

    public IEnumerable<SettingsSection> Sections { get; }

}
