using Avalonia.Threading;
using Soundboword.Inputs;
using Tmds.DBus.Protocol;

namespace Soundboword.Linux.Services;

public sealed class GlobalShortcutsInput : IInputMethod
{

    public const string Name = "XDG Global Shortcuts";

    private static (string Id, Dictionary<string, VariantValue>) ToShortcut(ShortcutAction action) => (action.Id, new Dictionary<string, VariantValue>
    {
        {"description", action.Description}
    });

    private readonly ShortcutAssigner _assigner;

    private readonly IDisposable _disposable;
    private readonly ShortcutList _list;
    private readonly GlobalShortcutsPortal _portal;

    private CancellationTokenSource? _cts;

    public GlobalShortcutsInput(IDisposable disposable, GlobalShortcutsPortal portal, ShortcutList list)
    {
        _disposable = disposable;
        _portal = portal;
        _list = list;
        _assigner = list.Assigner;
        _assigner.PropertyChanged += AssignerOnPropertyChanged;
    }

    public void Dispose()
    {
        _assigner.PropertyChanged -= AssignerOnPropertyChanged;
        _cts?.Dispose();
        _cts = null;
        _disposable.Dispose();
    }

    private void AssignerOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(ShortcutAssigner.IsAssigning))
            return;
        if (_assigner.IsAssigning)
        {
            _ = AssignAsync();
            return;
        }

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    private async Task AssignAsync()
    {
        if (_assigner.Target is not { } action)
            return;
        _cts = new CancellationTokenSource();
        var list = new List<(string, Dictionary<string, VariantValue>)>();
        var found = false;
        foreach (var shortcut in _list.ForRepository(Name))
        {
            if (shortcut.Action.Id == action.Id)
                found = true;
            list.Add(ToShortcut(shortcut.Action));
        }

        if (!found)
            list.Insert(0, ToShortcut(action));
        var sessionHandle = await _portal.SessionHandle.ConfigureAwait(false);
        var parentWindow = _portal.ParentWindow;
        var (response, results) = await _portal.RequestAsync((shortcuts, options) => shortcuts.BindShortcutsAsync(sessionHandle, list.ToArray(), parentWindow, options), _cts.Token).ConfigureAwait(false);
        if (response != 0)
            return;
        Dispatcher.UIThread.Post(() => _assigner.IsAssigning = false);
        var map = GlobalShortcutsRepository.CreateShortcutMap(results);
        if (map.TryGetValue(action.Id, out var description))
            Dispatcher.UIThread.Post(() => _list.Assign(description, action));
    }

}
