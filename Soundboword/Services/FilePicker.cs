using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Soundboword.Services;

public sealed partial class FilePicker : ObservableObject
{

    private readonly TopLevel? _topLevel;

    public FilePicker(TopLevel? topLevel = null) => _topLevel = topLevel;

    [ObservableProperty]
    public partial bool IsBrowsing { get; private set; }

    public async Task<string?> PickOne(FilePickerOpenOptions options)
    {
        if (_topLevel is not {StorageProvider: var provider})
            return null;
        options.AllowMultiple = false;
        IsBrowsing = true;
        try
        {
            var files = await provider.OpenFilePickerAsync(options);
            return files.Count == 0 ? null : files[0].TryGetLocalPath();
        }
        finally
        {
            MarkNotBrowsing();
        }
    }

    public async Task<IReadOnlyList<string>> PickMany(FilePickerOpenOptions options)
    {
        if (_topLevel is not {StorageProvider: var provider})
            return [];
        options.AllowMultiple = true;
        IsBrowsing = true;
        try
        {
            var files = await provider.OpenFilePickerAsync(options);
            return files.Select(e => e.TryGetLocalPath()).Where(e => e != null).ToList()!;
        }
        finally
        {
            MarkNotBrowsing();
        }
    }

    // Multipilier reference?
    private void MarkNotBrowsing()
    {
        if (Dispatcher.UIThread.CheckAccess())
            IsBrowsing = false;
        else
            Dispatcher.UIThread.Post(() => IsBrowsing = false);
    }

}
