using Avalonia.Platform.Storage;
using Microsoft.Extensions.Logging.Abstractions;
using Soundboword.Settings;
using Preferences = Soundboword.Settings.General.Preferences;

namespace Soundboword.Services;

[RegisterSingleton]
public sealed partial class SoundList
{

    private const string FileName = "sounds";

    private static readonly FolderPickerOpenOptions FolderOptions = new() {Title = "Select folders to locate sounds in"};

    public static FilePickerOpenOptions Options { get; } = new()
    {
        Title = "Add sounds",
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

        var count = Sounds.Count - notFound;
        if (count != 0)
            LogLoaded(count);
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
        Span<char> extension = stackalloc char[3];
        var any = false;
        foreach (var path in paths)
        {
            if (Path.GetExtension(path.AsSpan()).TrimStart('.').ToLower(extension, null) == -1 || extension is not ("mp3" or "wav"))
            {
                LogSkipped(path);
                continue;
            }

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
        Mode = _preferences.DefaultTriggerMode,
        Interaction = _preferences.DefaultInteraction
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

    [RelayCommand]
    private void Rescan()
    {
        LogRescanning();
        var loaded = 0;
        var notFound = 0;
        foreach (var sound in Sounds)
        {
            if (sound.PlaybackState != SoundState.NotFound)
                continue;
            if (File.Exists(sound.Path))
            {
                sound.UpdatePlaybackState(SoundState.Stopped);
                loaded++;
                continue;
            }

            notFound++;
            LogNotFound(sound.Name, sound.Path);
        }

        if (loaded != 0)
            LogLoaded(loaded);
        if (notFound != 0)
            LogNotFound(notFound);
    }

    [RelayCommand]
    private async Task RelinkAll()
    {
        var folders = await _filePicker.PickFolders(FolderOptions);
        if (folders.Count == 0)
            return;
        LogRelinking();
        var restored = 0;
        var notFound = 0;
        foreach (var sound in Sounds)
        {
            if (sound.PlaybackState != SoundState.NotFound)
                continue;
            if (File.Exists(sound.Path))
            {
                sound.UpdatePlaybackState(SoundState.Stopped);
                restored++;
                continue;
            }

            var filename = Path.GetFileName(sound.Path);
            var oldPath = sound.Path;
            foreach (var folder in folders)
            {
                var newPath = Path.Combine(folder, filename);
                if (!File.Exists(newPath))
                    continue;
                restored++;
                sound.Path = newPath;
                sound.UpdatePlaybackState(SoundState.Stopped);
                LogRestored(sound.Name, oldPath, newPath);
                break;
            }

            if (sound.Path == oldPath)
                notFound++;
        }

        if (restored != 0)
            LogRestored(restored);
        if (notFound != 0)
            LogNotFound(notFound);
        SaveSounds();
    }

    [LoggerMessage(LogLevel.Information, "Found {Count} sound(s)")]
    private partial void LogLoaded(int count);

    [LoggerMessage(LogLevel.Information, "Skipped adding a file because the extension was incompatible\nFile path: {Path}")]
    private partial void LogSkipped(string path);

    [LoggerMessage(LogLevel.Warning, "Could not load sound \"{Name}\"\nFile not found: {Path}")]
    private partial void LogNotFound(string name, string path);

    [LoggerMessage(LogLevel.Warning, "Could not find {Count} sound(s)")]
    private partial void LogNotFound(int count);

    [LoggerMessage(LogLevel.Information, "Rescanning sounds that could not be loaded")]
    private partial void LogRescanning();

    [LoggerMessage(LogLevel.Information, "Relinking all sounds that could not be found")]
    private partial void LogRelinking();

    [LoggerMessage(LogLevel.Information, "Restored sound \"{Name}\"\nNew location: {NewPath}\nOld location: {OldPath}")]
    private partial void LogRestored(string name, string oldPath, string newPath);

    [LoggerMessage(LogLevel.Information, "Restored {Count} sound(s)")]
    private partial void LogRestored(int count);

}
