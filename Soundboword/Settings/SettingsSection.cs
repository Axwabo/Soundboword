namespace Soundboword.Settings;

public abstract class SettingsSection : ViewModelBase
{

    public abstract string Title { get; }

    public abstract void Save();

}
