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

    public ShortcutAssigner ShortcutAssigner { get; }

    [ObservableProperty]
    public partial IBrush PressedBrush { get; set; } = Brushes.Gray;

    public MainWindowViewModel()
    {
        Board = new BoardViewModel();
        Playbacks = new PlaybacksViewModel();
        Inputs = new InputsViewModel();
        FilePicker = new FilePicker();
        ShortcutAssigner = new ShortcutAssigner();
    }

    public MainWindowViewModel(BoardViewModel board, PlaybacksViewModel playbacks, InputsViewModel inputs, FilePicker filePicker, ShortcutAssigner shortcutAssigner)
    {
        Board = board;
        Playbacks = playbacks;
        Inputs = inputs;
        FilePicker = filePicker;
        ShortcutAssigner = shortcutAssigner;
        Task.Run(TestDBus);
    }

    // TODO: upgrade Tmds.DBus when a new Avalonia version is released (new version included in Avalonia master 2 days ago)
    private async Task TestDBus()
    {
        try
        {
            // if (TopLevel.GetTopLevel(_host?.Host)?.TryGetPlatformHandle() is not {Handle: var windowHandle, HandleDescriptor: var handleDescriptor})
            // return;
            using var connection = new DBusConnection(DBusAddress.Session!);
            await connection.ConnectAsync();
            const string path = "/org/freedesktop/portal/desktop";
            var sender = (connection.UniqueName ?? "").TrimStart(':').Replace(".", "_");
            var handleToken = $"${path}/request/{sender}/Soundboword_{Guid.CreateVersion7()}";
            var sessionHandleToken = $"${path}/request/{sender}/Soundboword_{Guid.CreateVersion7()}";
            var token = "Soundboword_" + Stopwatch.GetTimestamp();
            var shortcutsProxy = new OrgFreedesktopPortalGlobalShortcutsProxy(connection.AsConnection(), "org.freedesktop.portal.Desktop", path);
            var session = await shortcutsProxy.CreateSessionAsync(new Dictionary<string, VariantValue>
            {
                {"handle_token", handleToken},
                {"session_handle_token", sessionHandleToken}
            });
            var requestHandle = await shortcutsProxy.BindShortcutsAsync(session, [
                ("Activate", new Dictionary<string, VariantValue>
                {
                    {"description", "Amogus activation"}
                })
            ], "", []);
            await shortcutsProxy.WatchActivatedAsync(((exception, tuple) => Console.WriteLine("activated")));
            Console.WriteLine();
            // var session = await shortcuts.CreateSessionAsync(path, path, "Soundboword", []);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

}
