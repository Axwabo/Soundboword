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

    private readonly FilePicker _filePicker;

    public SoundList()
    {
        AudioManager = new AudioManager(new SoundFlowDeviceManager());
        _filePicker = new FilePicker();
        Editor = new SoundEditingContext();
    }

    public SoundList(FilePicker filePicker, IClassicDesktopStyleApplicationLifetime? lifetime, SoundEditingContext editor, AudioManager audioManager)
    {
        _filePicker = filePicker;
        Editor = editor;
        AudioManager = audioManager;
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

    public static FilePickerOpenOptions Options { get; } = new()
    {
        Title = "Pick a sound",
        FileTypeFilter =
        [
            new FilePickerFileType("Audio files (mp3, wav)")
            {
                Patterns = ["*.mp3", "*.wav"],
                MimeTypes = ["audio/mpeg", "audio/wav"]
            }
        ]
    };

    public AudioManager AudioManager { get; }

    public SoundEditingContext Editor { get; }

    public ObservableCollection<SoundViewModel> Sounds { get; } = [];

    [RelayCommand]
    private async Task Add()
    {
        var list = await _filePicker.PickMany(Options);
        if (list.Count == 0)
            return;
        foreach (var path in list)
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
