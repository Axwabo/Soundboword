using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using Soundboword.Models;
using Soundboword.ViewModels;

namespace Soundboword;

public static class UserData
{

    private static readonly string Folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Soundboword");

    private static readonly string Sounds = Path.Combine(Folder, "sounds.json");

    private static void EnsureDirectory() => Directory.CreateDirectory(Folder);

    public static IEnumerable<SoundDto> LoadSounds()
    {
        EnsureDirectory();
        if (!File.Exists(Sounds))
            return [];
        try
        {
            using var file = File.OpenRead(Sounds);
            return JsonSerializer.Deserialize<IEnumerable<SoundDto>>(file, SourceGenerationContext.Default.IEnumerableSoundDto) ?? [];
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
        JsonSerializer.Serialize(file, sounds.Select(e => new SoundDto(e.Name, e.Path, e.Mode, e.Loop)), SourceGenerationContext.Default.IEnumerableSoundDto);
    }

}
