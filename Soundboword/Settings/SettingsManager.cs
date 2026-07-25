namespace Soundboword.Settings;

public sealed class SettingsManager : ViewModelBase
{

    public SettingsManager(IEnumerable<SettingsSection> sections) => Sections = sections.ToList();

    public List<SettingsSection> Sections { get; }

    public T Require<T>() => Sections.OfType<T>().First();

}
