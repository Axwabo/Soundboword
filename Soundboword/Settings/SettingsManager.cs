using Soundboword.Settings.General;

namespace Soundboword.Settings;

public sealed class SettingsManager : ViewModelBase
{

    public SettingsManager() : this([new PreferencesProvider(new Preferences())])
    {
    }

    public SettingsManager(IEnumerable<ISettingsProvider> providers)
    {
        Sections = providers.SelectMany(e => e.Sections).ToList();
        LastSection = Sections[^1];
    }

    public List<SettingsSection> Sections { get; }

    public SettingsSection? LastSection { get; }

    public T Require<T>() => Sections.OfType<T>().First();

}
