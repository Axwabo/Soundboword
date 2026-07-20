using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Soundboword.Settings;

namespace Soundboword.Services;

[RegisterSingleton]
public sealed partial class SoundList
{

    private const string FileName = "sounds";

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

    private readonly FilePicker _filePicker;

    private readonly Preferences _preferences;

    public SoundList()
    {
        AudioManager = new AudioManager(new SoundFlowDeviceManager());
        _filePicker = new FilePicker();
        _preferences = new Preferences();
        Editor = new SoundEditingContext(_filePicker);
    }

    public SoundList(FilePicker filePicker, Preferences preferences, IClassicDesktopStyleApplicationLifetime? lifetime, SoundEditingContext editor, AudioManager audioManager)
    {
        _filePicker = filePicker;
        _preferences = preferences;
        Editor = editor;
        AudioManager = audioManager;
        foreach (var sound in UserData.Load(FileName, () => [], SourceGenerationContext.Default.IEnumerableSoundDto))
        {
            var soundViewModel = new SoundViewModel
            {
                Id = sound.Id,
                Name = sound.Name,
                Path = sound.Path,
                Loop = sound.Loop,
                Volume = sound.Volume,
                Mode = sound.Mode,
                Interaction = sound.Interaction,
                List = this
            };
            if (!File.Exists(soundViewModel.Path))
                soundViewModel.UpdatePlaybackState(SoundState.NotFound);
            Sounds.Add(soundViewModel);
        }

        lifetime?.Exit += (_, _) => SaveSounds();
    }

    public AudioManager AudioManager { get; }

    public SoundEditingContext Editor { get; }

    public ObservableCollection<SoundViewModel> Sounds { get; } = [];

    [RelayCommand]
    private async Task Add() => Add(await _filePicker.PickMany(Options));

    public void Add(IEnumerable<string> paths)
    {
        var any = false;
        foreach (var path in paths)
        {
            any = true;
            Add(path, Path.GetFileNameWithoutExtension(path));
        }

        if (any)
            SaveSounds();
    }

    public void Add(string path, string name) => Sounds.Add(new SoundViewModel
    {
        Id = Guid.NewGuid(),
        Path = path,
        Name = name,
        List = this,
        Mode = _preferences.DefaultTriggerMode
    });

    public void Delete(SoundViewModel sound)
    {
        Sounds.Remove(sound);
        SaveSounds();
    }

    public void SaveSounds() => UserData.Save(
        FileName,
        Sounds.Select(e => new SoundDto(e.Id, e.Name, e.Path, e.Mode, e.Loop, e.Volume, e.Interaction)),
        SourceGenerationContext.Default.IEnumerableSoundDto
    );

}
