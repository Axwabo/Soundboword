namespace Soundboword.Settings;

public interface ISettingsProvider
{

    IEnumerable<SettingsSection> Sections { get; }

}
