using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using Soundboword.Models;
using Soundboword.ViewModels;

namespace Soundboword.Services;

public sealed partial class SoundList
{

    private static readonly FilePickerOpenOptions FileOptions = new()
    {
        Title = "Pick a sound",
        FileTypeFilter =
        [
            new FilePickerFileType("Audio files")
            {
                Patterns = ["*.mp3", "*.wav"],
                MimeTypes = ["audio/mpeg", "audio/wav"]
            }
        ]
    };

    private readonly TopLevel? _topLevel;

    public SoundEditingContext Editor { get; }

    public ObservableCollection<SoundViewModel> Sounds { get; } = [];

    public SoundList() => Editor = new SoundEditingContext();

    public SoundList(TopLevel? topLevel, IClassicDesktopStyleApplicationLifetime? lifetime, SoundEditingContext editor)
    {
        _topLevel = topLevel;
        Editor = editor;
        foreach (var sound in UserData.LoadSounds())
        {
            var soundViewModel = new SoundViewModel
            {
                Id = sound.Id,
                Name = sound.Name,
                Path = sound.Path,
                Loop = sound.Loop,
                Volume = sound.Volume,
                Mode = sound.Mode,
                List = this,
            };
            if (!File.Exists(soundViewModel.Path))
                soundViewModel.UpdatePlaybackState(SoundState.Error);
            Sounds.Add(soundViewModel);
        }

        lifetime?.Exit += (_, _) => UserData.SaveSounds(Sounds);
    }

    [RelayCommand]
    private async Task Add()
    {
        var path = await BrowseAudioAsync(_topLevel);
        if (path == null)
            return;
        Sounds.Add(new SoundViewModel
        {
            Id = Guid.NewGuid(),
            Path = path,
            Name = Path.GetFileNameWithoutExtension(path),
            List = this
        });
        UserData.SaveSounds(Sounds);
    }

    public void Delete(SoundViewModel sound)
    {
        Sounds.Remove(sound);
        UserData.SaveSounds(Sounds);
    }

    public static async Task<string?> BrowseAudioAsync(TopLevel? topLevel)
    {
        if (topLevel is not {StorageProvider: var provider})
            return null;
        var files = await provider.OpenFilePickerAsync(FileOptions);
        return files.Count == 0 ? null : files[0].TryGetLocalPath()!;
    }

}
