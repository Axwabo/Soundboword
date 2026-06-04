using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Soundboword.Inputs;
using Soundboword.Models;
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
    private readonly IFileManagerOpener? _opener;

    public InputsViewModel Inputs { get; }

    public ObservableCollection<SoundViewModel> Sounds { get; } = [];

    [ObservableProperty]
    public partial IBrush PressedBrush { get; set; } = Brushes.Gray;

    public MainWindowViewModel() => Inputs = new InputsViewModel();

    public MainWindowViewModel(HostControl host, IFileManagerOpener opener, IEnumerable<IInputFactory> factories)
    {
        _host = host;
        _opener = opener;
        Inputs = new InputsViewModel(factories);
        foreach (var sound in UserData.LoadSounds())
            Sounds.Add(new SoundViewModel
            {
                Name = sound.Name,
                Path = sound.Path,
                Loop = sound.Loop,
                Mode = sound.Mode,
                Opener = opener
            });
        _ = TestDBus().ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task AddSound()
    {
        if (_host?.Host is not {StorageProvider: var provider})
            return;
        var files = await provider.OpenFilePickerAsync(FileOptions);
        if (files.Count == 0)
            return;
        Sounds.Add(new SoundViewModel
        {
            Path = files[0].TryGetLocalPath()!,
            Name = Path.GetFileNameWithoutExtension(files[0].TryGetLocalPath()!),
            Opener = _opener
        });
        UserData.SaveSounds(Sounds.Select(e => new SoundDto(e.Name, e.Path, e.Mode, e.Loop)));
    }

    [RelayCommand]
    private static void StopAll() => AudioManager.StopAll();

    // TODO: upgrade Tmds.DBus when a new Avalonia version is released (new version included in Avalonia master 2 days ago)
    private async Task TestDBus()
    {
        // if (TopLevel.GetTopLevel(_host?.Host)?.TryGetPlatformHandle() is not {Handle: var windowHandle, HandleDescriptor: var handleDescriptor})
        // return;
        using var connection = new DBusConnection(DBusAddress.Session!);
        await connection.ConnectAsync();
        var sender = (connection.UniqueName ?? "").TrimStart(':').Replace(".", "_");
        var token = "Soundboword_" + Stopwatch.GetTimestamp();
        string path = $"/org/freedesktop/portal/desktop/request/{sender}/{token}";
        var proxy = new OrgFreedesktopPortalGlobalShortcutsProxy(connection.AsConnection(), "org.freedesktop.portal.GlobalShortcuts", path);
        var session = await proxy.CreateSessionAsync([]);
        var requestHandle = await proxy.BindShortcutsAsync(session, [
            ("Activate", new Dictionary<string, VariantValue>
            {
                {"description", "Amogus activation"}
            })
        ], "", []);
        await proxy.WatchActivatedAsync(((exception, tuple) => Console.WriteLine("activated")));
        Console.WriteLine();
        // var session = await shortcuts.CreateSessionAsync(path, path, "Soundboword", []);
    }

}
