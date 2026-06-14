using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Soundboword;

public static class UserData
{

    public static string Folder { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Soundboword");

    private static string FullPath(string name) => Path.Combine(Folder, $"{name}.json");

    private static void EnsureDirectory() => Directory.CreateDirectory(Folder);

    public static string? Load(string name)
    {
        EnsureDirectory();
        var path = FullPath(name);
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
            File.WriteAllText(FullPath(name), content);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public static T Load<T>(string name, Func<T> fallback, JsonTypeInfo<T> typeInfo) where T : notnull
    {
        EnsureDirectory();
        var path = FullPath(name);
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
            using var file = File.Create(FullPath(name));
            JsonSerializer.Serialize(file, data, typeInfo);
        }
        catch (Exception)
        {
            // ignored
            // TODO: log somehow
        }
    }

}
