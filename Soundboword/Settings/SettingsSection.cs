using System.Text.Json.Serialization.Metadata;

namespace Soundboword.Settings;

public abstract class SettingsSection : ViewModelBase
{

    private const string Filename = "settings";

    private readonly UserData? _data;

    protected SettingsSection(UserData? data = null) => _data = data;

    public abstract void Save();

    protected T Load<T>(Func<T> fallback, JsonTypeInfo<T> info, string filename = Filename) where T : notnull => _data!.Load(filename, fallback, info);

    protected void Save<T>(T data, JsonTypeInfo<T> info, string filename = Filename) where T : notnull => _data!.Save(filename, data, info);

}
