using Soundboword.Settings;

namespace Soundboword.Linux.PipeWire.Settings;

[RegisterSingleton<ISettingsProvider>(Duplicate = DuplicateStrategy.Append)]
public sealed class PreferencesProvider : ISettingsProvider
{

    public PreferencesProvider(PipeWirePreferences preferences) => Sections = [preferences];

    public IEnumerable<SettingsSection> Sections { get; }

}
