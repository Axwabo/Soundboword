using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Soundboword.Inputs.Launchpad;
using Soundboword.Models;
using Soundboword.ViewModels;

namespace Soundboword;

public static class UserData
{

    private static readonly JsonSerializerOptions Options = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters =
        {
            new JsonStringEnumConverter<PlaybackMode>(),
            new JsonStringEnumConverter<LaunchpadKey>(),
        }
    };

    public static string Folder { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Soundboword");

    private static string Sounds { get; } = Path.Combine(Folder, "sounds.json");

    private static void EnsureDirectory() => Directory.CreateDirectory(Folder);

    private static T Load<T>(string path, Func<T> fallback) where T : notnull
    {
        EnsureDirectory();
        if (!File.Exists(path))
            return fallback();
        try
        {
            using var file = File.OpenRead(path);
            return JsonSerializer.Deserialize<T>(file, Options) ?? fallback();
        }
        catch (Exception)
        {
            return fallback();
        }
    }

    private static void Save<T>(string path, T data) where T : notnull
    {
        EnsureDirectory();
        try
        {
            using var file = File.Create(path);
            JsonSerializer.Serialize(file, data, Options);
        }
        catch (Exception)
        {
            // ignored
            // TODO: log somehow
        }
    }

    public static IReadOnlyList<SoundDto> LoadSounds() => Load<IReadOnlyList<SoundDto>>(Sounds, () => []);

    public static void SaveSounds(ObservableCollection<SoundViewModel> sounds) => Save(Sounds, sounds.Select(e => new SoundDto(e.Id, e.Name, e.Path, e.Mode, e.Loop)));

}
