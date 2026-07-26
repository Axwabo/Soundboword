using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.Logging.Abstractions;

namespace Soundboword;

[RegisterSingleton]
public sealed partial class UserData
{

    public static string Root { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Soundboword");

    private readonly ILogger _logger;

    public UserData(ILoggerFactory? loggerFactory = null)
    {
        Folder = Root;
        _logger = loggerFactory?.CreateLogger("User Data") ?? NullLogger.Instance;
    }

    public UserData(string folder, ILoggerFactory loggerFactory)
    {
        Folder = Path.Combine(Root, folder);
        _logger = loggerFactory.CreateLogger($"{folder} Data");
    }

    public string Folder { get; }

    private string FullPath(string name, bool json) => Path.Combine(Folder, $"{name}.{(json ? "json" : "txt")}");

    private void EnsureDirectory() => Directory.CreateDirectory(Folder);

    public string? Load(string name)
    {
        EnsureDirectory();
        var path = FullPath(name, false);
        if (!File.Exists(path))
            return null;
        try
        {
            return File.ReadAllText(path);
        }
        catch (Exception e)
        {
            LogReadFailure(name, e);
            return null;
        }
    }

    public void Save(string name, string content)
    {
        EnsureDirectory();
        try
        {
            File.WriteAllText(FullPath(name, false), content);
        }
        catch (Exception e)
        {
            LogWriteFailure(name, e);
        }
    }

    public T Load<T>(string name, Func<T> fallback, JsonTypeInfo<T>? typeInfo) where T : notnull
    {
        if (typeInfo == null)
            return fallback();
        EnsureDirectory();
        var path = FullPath(name, true);
        if (!File.Exists(path))
            return fallback();
        try
        {
            using var file = File.OpenRead(path);
            return JsonSerializer.Deserialize(file, typeInfo) ?? fallback();
        }
        catch (Exception e)
        {
            LogReadFailure(name, e);
            return fallback();
        }
    }

    public void Save<T>(string name, T data, JsonTypeInfo<T>? typeInfo) where T : notnull
    {
        if (typeInfo == null)
            return;
        EnsureDirectory();
        try
        {
            using var file = File.Create(FullPath(name, true));
            JsonSerializer.Serialize(file, data, typeInfo);
        }
        catch (Exception e)
        {
            LogWriteFailure(name, e);
        }
    }

    [LoggerMessage(LogLevel.Error, "Failed to load {File}")]
    private partial void LogReadFailure(string file, Exception exception);

    [LoggerMessage(LogLevel.Error, "Failed to save {File}")]
    private partial void LogWriteFailure(string file, Exception exception);

}
