using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
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

    private readonly HostControl? _host;
    private readonly IFileManagerOpener? _opener;

    public ObservableCollection<SoundViewModel> Sounds { get; } = [];

    public SoundList()
    {
    }

    public SoundList(HostControl? host, IFileManagerOpener? opener)
    {
        _host = host;
        _opener = opener;
        foreach (var sound in UserData.LoadSounds())
            Sounds.Add(new SoundViewModel
            {
                Name = sound.Name,
                Path = sound.Path,
                Loop = sound.Loop,
                Mode = sound.Mode,
                Opener = opener,
                List = this
            });
    }

    [RelayCommand]
    private async Task Add()
    {
        if (_host?.Host is not {StorageProvider: var provider})
            return;
        var files = await provider.OpenFilePickerAsync(FileOptions);
        if (files.Count == 0)
            return;
        var path = files[0].TryGetLocalPath()!;
        Sounds.Add(new SoundViewModel
        {
            Path = path,
            Name = Path.GetFileNameWithoutExtension(path),
            Opener = _opener,
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
