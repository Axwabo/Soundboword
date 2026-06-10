using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Soundboword.Models;
using Soundboword.Services;

namespace Soundboword.ViewModels;

public sealed partial class EditSoundViewModel : ViewModelBase
{

    private readonly TopLevel? _topLevel;
    private readonly FilePicker _filePicker;
    private readonly IFileManagerOpener? _opener;
    private readonly ShortcutList _shortcuts;
    private readonly AudioManager _audioManager;

    public SoundEditingContext Context { get; }

    public ObservableCollection<Shortcut> Active { get; } = [];

    public EditSoundViewModel()
    {
        Context = new SoundEditingContext();
        _filePicker = new FilePicker();
        _audioManager = new AudioManager();
        _shortcuts = new ShortcutList(null, new SoundList(_filePicker, null, Context, _audioManager));
    }

    public EditSoundViewModel(TopLevel topLevel, FilePicker filePicker, IFileManagerOpener opener, SoundEditingContext context, ShortcutList shortcuts, AudioManager audioManager)
    {
        _topLevel = topLevel;
        _filePicker = filePicker;
        _opener = opener;
        _shortcuts = shortcuts;
        _audioManager = audioManager;
        Context = context;
        Context.PropertyChanged += ContextOnPropertyChanged;
        _shortcuts.ShortcutsChanged += ShortcutsOnShortcutsChanged;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MaxVolume))]
    public partial bool RaiseMaximumVolume { get; set; }

    public float MaxVolume => RaiseMaximumVolume ? 2 : 1;

    [ObservableProperty]
    public partial bool IsNotFound { get; private set; }

    [RelayCommand]
    private void Stop()
    {
        if (Context.Model != null)
            _audioManager.StopAll(Context.Model);
    }

    [RelayCommand]
    private async Task Relink()
    {
        if (Context.Model is not { } model)
            return;
        var path = await _filePicker.PickOne(SoundList.Options);
        if (path == null)
            return;
        model.Path = path;
        IsNotFound = false;
    }

    [RelayCommand]
    private void TogglePause()
    {
        if (Context.Model != null)
            _audioManager.TogglePause(Context.Model);
    }

    [RelayCommand]
    private void CopyPath()
    {
        if (Context.Model is {Path: var path} && _topLevel is {Clipboard: { } clipboard})
            clipboard.SetTextAsync(path);
    }

    [RelayCommand]
    private void Reveal()
    {
        if (Context.Model is {Path: var path})
            _opener?.Open(path);
    }

    [RelayCommand]
    private void Delete()
    {
        if (Context.Model is not { } model)
            return;
        Context.Close();
        model.List.Delete(model);
        _shortcuts.Remove(model);
    }

    [RelayCommand]
    private void RemoveShortcuts()
    {
        if (Context.Model is { } model)
            _shortcuts.Remove(model);
    }

    private void ContextOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(SoundEditingContext.Model))
            return;
        UpdateActiveShortcuts();
        IsNotFound = Context.Model is { } model && !File.Exists(model.Path);
        RaiseMaximumVolume |= Context.Volume > 1;
    }

    private void ShortcutsOnShortcutsChanged() => Dispatcher.UIThread.Post(UpdateActiveShortcuts);

    private void UpdateActiveShortcuts()
    {
        Active.Clear();
        if (Context.Model is not { } model)
            return;
        foreach (var shortcut in _shortcuts.ForSound(model))
            Active.Add(shortcut);
    }

}
