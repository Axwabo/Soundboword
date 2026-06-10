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
    private readonly IFileManagerOpener? _opener;
    private readonly ShortcutList _shortcuts;

    public SoundEditingContext Context { get; }

    public ObservableCollection<Shortcut> Active { get; } = [];

    public EditSoundViewModel()
    {
        Context = new SoundEditingContext();
        _shortcuts = new ShortcutList(null, new SoundList(_topLevel, null, Context));
    }

    public EditSoundViewModel(TopLevel topLevel, IFileManagerOpener opener, SoundEditingContext context, ShortcutList shortcuts)
    {
        _topLevel = topLevel;
        _opener = opener;
        _shortcuts = shortcuts;
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
            AudioManager.StopAll(Context.Model);
    }

    [RelayCommand]
    private async Task Relink()
    {
        if (Context.Model is not { } model)
            return;
        var path = await SoundList.BrowseAudioAsync(_topLevel);
        if (path == null)
            return;
        model.Path = path;
        IsNotFound = false;
    }

    [RelayCommand]
    private void TogglePause()
    {
        if (Context.Model != null)
            AudioManager.TogglePause(Context.Model);
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
