using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Soundboword.Models;
using Soundboword.ViewModels;

namespace Soundboword;

public static class UserData
{

    private static readonly JsonSerializerOptions Options = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = {new JsonStringEnumConverter<PlaybackMode>()}
    };

    private static readonly string Folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Soundboword");

    private static readonly string Sounds = Path.Combine(Folder, "sounds.json");

    private static void EnsureDirectory() => Directory.CreateDirectory(Folder);

    public static IReadOnlyList<SoundDto> LoadSounds()
    {
        EnsureDirectory();
        if (!File.Exists(Sounds))
            return [];
        try
        {
            using var file = File.OpenRead(Sounds);
            return JsonSerializer.Deserialize<IReadOnlyList<SoundDto>>(file, Options) ?? [];
        }
        catch (Exception)
        {
            return [];
        }
    }

    public static void SaveSounds(ObservableCollection<SoundViewModel> sounds)
    {
        EnsureDirectory();
        using var file = File.Create(Sounds);
        JsonSerializer.Serialize(file, sounds.Select(e => new SoundDto(e.Name, e.Path, e.Mode, e.Loop)), Options);
    }

}
