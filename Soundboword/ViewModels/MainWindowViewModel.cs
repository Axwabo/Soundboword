using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Soundboword.Services;
using Tmds.DBus.Protocol;
using Tmds.DBus.SourceGenerator;

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

    [ObservableProperty]
    public partial IBrush PressedBrush { get; set; } = Brushes.Gray;

    public MainWindowViewModel()
    {
    }

    public MainWindowViewModel(HostControl host)
    {
        _host = host;
        _ = TestDBus().ConfigureAwait(false);
    }

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

    // TODO: upgrade Tmds.DBus when a new Avalonia version is released (new version included in Avalonia master 2 days ago)
    private async Task TestDBus()
    {
        using var connection = new DBusConnection(DBusAddress.Session!);
        await connection.ConnectAsync();
        var sender = (connection.UniqueName ?? "").TrimStart(':').Replace(".", "_");
        var token = "Soundboword_" + Stopwatch.GetTimestamp();
        ObjectPath path = $"/org/freedesktop/portal/desktop/request/{sender}/{token}";
        var proxy = new OrgFreedesktopImplPortalGlobalShortcutsProxy(connection.AsConnection(), "org.freedesktop.portal.GlobalShortcuts", "/org/greedesktop/portal/desktop");
        var (sessionResponse, whatever) = await proxy.CreateSessionAsync(path, $"/org/freedesktop/portal/desktop/session/{sender}/{token}", "Soundboword", []);
        Console.WriteLine();
        // var session = await shortcuts.CreateSessionAsync(path, path, "Soundboword", []);
    }

}
