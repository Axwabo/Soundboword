using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Soundboword.Services;

public sealed partial class FilePicker : ObservableObject
{

    private readonly TopLevel? _topLevel;

    [ObservableProperty]
    public partial bool IsBrowsing { get; private set; }

    public FilePicker(TopLevel? topLevel = null) => _topLevel = topLevel;

    public async Task<string?> PickOne(FilePickerOpenOptions options)
    {
        if (_topLevel is not {StorageProvider: var provider})
            return null;
        IsBrowsing = true;
        try
        {
            var files = await provider.OpenFilePickerAsync(options);
            return files.Count == 0 ? null : files[0].TryGetLocalPath();
        }
        finally
        {
            if (Dispatcher.UIThread.CheckAccess())
                IsBrowsing = false;
            else
                Dispatcher.UIThread.Post(() => IsBrowsing = false);
        }
    }

}
