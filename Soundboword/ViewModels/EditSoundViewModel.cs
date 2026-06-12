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
    private readonly AudioManager _audioManager;

    public ShortcutList Shortcuts { get; }

    public SoundEditingContext Context { get; }

    public ObservableCollection<Shortcut> Active { get; } = [];

    public EditSoundViewModel()
    {
        Context = new SoundEditingContext();
        _filePicker = new FilePicker();
        _audioManager = new AudioManager(new SoundFlowDeviceManager());
        Shortcuts = new ShortcutList(null, new SoundList(_filePicker, null, Context, _audioManager), new ShortcutAssigner());
    }

    public EditSoundViewModel(TopLevel topLevel, FilePicker filePicker, IFileManagerOpener opener, SoundEditingContext context, AudioManager audioManager, ShortcutList shortcuts)
    {
        _topLevel = topLevel;
        _filePicker = filePicker;
        _opener = opener;
        _audioManager = audioManager;
        Shortcuts = shortcuts;
        Context = context;
        Context.PropertyChanged += ContextOnPropertyChanged;
        Shortcuts.ShortcutsChanged += ShortcutsOnShortcutsChanged;
    }

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
        Shortcuts.Remove(new TriggerSoundAction(model));
    }

    [RelayCommand]
    private void RemoveShortcuts()
    {
        if (Context.Model is { } model)
            Shortcuts.Remove(new TriggerSoundAction(model));
    }

    private void ContextOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(SoundEditingContext.Model))
            return;
        UpdateActiveShortcuts();
        IsNotFound = Context.Model is { } model && !File.Exists(model.Path);
    }

    private void ShortcutsOnShortcutsChanged() => Dispatcher.UIThread.Post(UpdateActiveShortcuts);

    private void UpdateActiveShortcuts()
    {
        Active.Clear();
        if (Context.Model is not { } model)
            return;
        foreach (var shortcut in Shortcuts.ForSound(model))
            Active.Add(shortcut);
    }

}
