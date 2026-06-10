using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using Soundboword.Models;
using Soundboword.ViewModels;

namespace Soundboword.Services;

public sealed partial class SoundList
{

    public static FilePickerOpenOptions Options { get; } = new()
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

    private readonly FilePicker _filePicker;

    public SoundEditingContext Editor { get; }

    public ObservableCollection<SoundViewModel> Sounds { get; } = [];

    public SoundList()
    {
        _filePicker = new FilePicker();
        Editor = new SoundEditingContext();
    }

    public SoundList(FilePicker filePicker, IClassicDesktopStyleApplicationLifetime? lifetime, SoundEditingContext editor)
    {
        _filePicker = filePicker;
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
        var path = await _filePicker.PickOne(Options);
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

}
