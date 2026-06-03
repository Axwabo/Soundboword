using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using Soundboword.Services;

namespace Soundboword.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
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

    public ObservableCollection<SoundViewModel> Sounds { get; } = [];

    public MainWindowViewModel()
    {
    }

    public MainWindowViewModel(HostControl host) => _host = host;

    [RelayCommand]
    private async Task AddSound()
    {
        if (TopLevel.GetTopLevel(_host?.Host) is not {StorageProvider: var provider})
            return;
        var files = await provider.OpenFilePickerAsync(FileOptions);
        if (files.Count == 0)
            return;
        Sounds.Add(new SoundViewModel
        {
            Path = files[0].TryGetLocalPath()!,
            Name = Path.GetFileNameWithoutExtension(files[0].TryGetLocalPath()!)
        });
    }

}
