using Avalonia.Platform.Storage;
using Microsoft.Extensions.Logging.Abstractions;
using Soundboword.Settings;
using Preferences = Soundboword.Settings.General.Preferences;

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

    private readonly UserData _data;
    private readonly FilePicker _filePicker;
    private readonly ILogger _logger;
    private readonly Preferences _preferences;

    public SoundList()
    {
        _data = new UserData();
        AudioManager = new AudioManager(new SoundFlowDeviceManager(_data, new Lifetime()));
        _filePicker = new FilePicker();
        _logger = NullLogger.Instance;
        _preferences = new Preferences();
        Editor = new SoundEditingContext(_filePicker);
    }

    public SoundList(UserData data, FilePicker filePicker, ILoggerFactory loggerFactory, SettingsManager settingsManager, Lifetime lifetime, SoundEditingContext editor, AudioManager audioManager)
    {
        _data = data;
        _filePicker = filePicker;
        _logger = loggerFactory.CreateLogger("Sounds");
        _preferences = settingsManager.Require<Preferences>();
        Editor = editor;
        AudioManager = audioManager;
        var notFound = 0;
        foreach (var sound in _data.Load(FileName, () => [], SourceGenerationContext.Default.IEnumerableSoundDto))
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
            Sounds.Add(soundViewModel);
            if (File.Exists(soundViewModel.Path))
                continue;
            LogNotFound(sound.Name, sound.Path);
            soundViewModel.UpdatePlaybackState(SoundState.NotFound);
            notFound++;
        }

        if (notFound != 0)
            LogNotFound(notFound);
        lifetime.Exit += SaveSounds;
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

    public void SaveSounds() => _data.Save(
        FileName,
        Sounds.Select(e => new SoundDto(e.Id, e.Name, e.Path, e.Mode, e.Loop, e.Volume, e.Interaction)),
        SourceGenerationContext.Default.IEnumerableSoundDto
    );

    [LoggerMessage(LogLevel.Warning, "Could not load sound {Name}\nFile not found: {Path}")]
    private partial void LogNotFound(string name, string path);

    [LoggerMessage(LogLevel.Warning, "Could not find {Count} sound(s)")]
    private partial void LogNotFound(int count);

}
