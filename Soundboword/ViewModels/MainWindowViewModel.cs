using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Soundboword.Services;
using Tmds.DBus.Protocol;
using Tmds.DBus.SourceGenerator;

namespace Soundboword.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{

    public BoardViewModel Board { get; }

    public PlaybacksViewModel Playbacks { get; }

    public InputsViewModel Inputs { get; }

    public FilePicker FilePicker { get; }

    [ObservableProperty]
    public partial IBrush PressedBrush { get; set; } = Brushes.Gray;

    public MainWindowViewModel()
    {
        Board = new BoardViewModel();
        Playbacks = new PlaybacksViewModel();
        Inputs = new InputsViewModel();
        FilePicker = new FilePicker();
    }

    public MainWindowViewModel(BoardViewModel board, PlaybacksViewModel playbacks, InputsViewModel inputs, FilePicker filePicker)
    {
        Board = board;
        Inputs = inputs;
        FilePicker = filePicker;
        Playbacks = playbacks;
        _ = TestDBus().ConfigureAwait(false);
    }

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
