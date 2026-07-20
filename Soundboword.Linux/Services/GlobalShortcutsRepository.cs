using Avalonia.Threading;
using Soundboword.Inputs;
using Tmds.DBus.Protocol;

namespace Soundboword.Linux.Services;

public sealed class GlobalShortcutsRepository : ShortcutRepository<string>
{

    public GlobalShortcutsRepository(GlobalShortcutsPortal portal, AudioManager audioManager, SoundList soundList) : base(audioManager,
        soundList,
        GlobalShortcutsInput.Name,
        e => e,
        null)
    {
        if (portal.IsAvailable)
            _ = LoadMapAsync(portal, soundList);
    }

    private async Task LoadMapAsync(GlobalShortcutsPortal portal, SoundList soundList)
    {
        var handle = await portal.SessionHandle.ConfigureAwait(false);
        var (response, results) = await portal.RequestAsync((shortcuts, options) => shortcuts.ListShortcutsAsync(handle, options)).ConfigureAwait(false);
        if (response != 0)
            return;
        var map = new Dictionary<string, string>();
        var shortcuts = results["shortcuts"].GetArray<VariantValue>();
        foreach (var value in shortcuts)
        {
            var id = value.GetItem(0).GetString();
            var properties = value.GetItem(1).GetDictionary<string, VariantValue>();
            var assigned = properties["trigger_description"].GetString();
            map[id] = assigned;
        }

        Dispatcher.UIThread.InvokeOrPost(() => InitializeMap(map, soundList));
    }

}
