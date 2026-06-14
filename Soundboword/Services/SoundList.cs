using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace Soundboword.Services;

public sealed partial class SoundList
{

    private const string FileName = "sounds";

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
                List = this,
            };
            if (!File.Exists(soundViewModel.Path))
                soundViewModel.UpdatePlaybackState(SoundState.Error);
            Sounds.Add(soundViewModel);
        }

        lifetime?.Exit += (_, _) => SaveSounds();
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
            Add(path, Path.GetFileNameWithoutExtension(path));
        SaveSounds();
    }

    public void Add(string path, string name) => Sounds.Add(new SoundViewModel
    {
        Id = Guid.NewGuid(),
        Path = path,
        Name = name,
        List = this
    });

    public void Delete(SoundViewModel sound)
    {
        Sounds.Remove(sound);
        SaveSounds();
    }

    public void SaveSounds() => UserData.Save(
        FileName,
        Sounds.Select(e => new SoundDto(e.Id, e.Name, e.Path, e.Mode, e.Loop, e.Volume)),
        SourceGenerationContext.Default.IEnumerableSoundDto
    );

}
