using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Soundboword.Services;
using Tmds.DBus.Protocol;
using Tmds.DBus.SourceGenerator;

namespace Soundboword.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{

    public InputsViewModel Inputs { get; }

    public SoundList SoundList { get; }

    public EditSoundViewModel Editor { get; }

    [ObservableProperty]
    public partial IBrush PressedBrush { get; set; } = Brushes.Gray;

    public MainWindowViewModel()
    {
        Inputs = new InputsViewModel();
        SoundList = new SoundList();
        Editor = new EditSoundViewModel();
    }

    public MainWindowViewModel(SoundList list, InputsViewModel inputs, EditSoundViewModel editor)
    {
        SoundList = list;
        Inputs = inputs;
        Editor = editor;
        _ = TestDBus().ConfigureAwait(false);
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
