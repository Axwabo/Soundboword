using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Soundboword;

public static class UserData
{

    private static readonly string Folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Soundboword");

    private static string FullPath(string name, bool json) => Path.Combine(Folder, $"{name}.{(json ? "json" : "txt")}");

    private static void EnsureDirectory() => Directory.CreateDirectory(Folder);

    public static string? Load(string name)
    {
        EnsureDirectory();
        var path = FullPath(name, false);
        if (!File.Exists(path))
            return null;
        try
        {
            return File.ReadAllText(path);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static void Save(string name, string content)
    {
        EnsureDirectory();
        try
        {
            File.WriteAllText(FullPath(name, false), content);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public static T Load<T>(string name, Func<T> fallback, JsonTypeInfo<T> typeInfo) where T : notnull
    {
        EnsureDirectory();
        var path = FullPath(name, true);
        if (!File.Exists(path))
            return fallback();
        try
        {
            using var file = File.OpenRead(path);
            return JsonSerializer.Deserialize(file, typeInfo) ?? fallback();
        }
        catch (Exception)
        {
            return fallback();
        }
    }

    public static void Save<T>(string name, T data, JsonTypeInfo<T> typeInfo) where T : notnull
    {
        EnsureDirectory();
        try
        {
            using var file = File.Create(FullPath(name, true));
            JsonSerializer.Serialize(file, data, typeInfo);
        }
        catch (Exception)
        {
            // ignored
            // TODO: log somehow
        }
    }

}
