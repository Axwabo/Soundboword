using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Input.Platform;
using CommunityToolkit.Mvvm.Input;
using Soundboword.Models;
using Soundboword.Services;

namespace Soundboword.ViewModels;

public sealed partial class EditSoundViewModel : ViewModelBase
{

    private readonly HostControl? _host;
    private readonly IFileManagerOpener? _opener;
    private readonly InputsViewModel _inputs;

    public SoundEditingContext Context { get; }

    public ObservableCollection<Shortcut> Active { get; } = [];

    public EditSoundViewModel()
    {
        _inputs = new InputsViewModel();
        Context = new SoundEditingContext();
    }

    public EditSoundViewModel(HostControl host, IFileManagerOpener opener, SoundEditingContext context, InputsViewModel inputs)
    {
        _host = host;
        _opener = opener;
        _inputs = inputs;
        Context = context;
        Context.PropertyChanged += ContextOnPropertyChanged;
    }

    [RelayCommand]
    private void Stop()
    {
        if (Context.Model != null)
            AudioManager.StopAll(Context.Model);
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
        if (Context.Model is {Path: var path} && _host is {Host.Clipboard: { } clipboard})
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
        ShortcutList.RemoveAll(model);
    }

    [RelayCommand]
    private void ToggleListening() => Context.ToggleListening(_inputs);

    [RelayCommand]
    private void RemoveShortcuts()
    {
        if (Context.Model is { } model)
            ShortcutList.RemoveAll(model);
    }

    private void ContextOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SoundEditingContext.Model))
            UpdateActiveShortcuts();
    }

    private void UpdateActiveShortcuts()
    {
        Active.Clear();
        if (Context.Model is not { } model)
            return;
        foreach (var shortcut in ShortcutList.ForSound(model))
            Active.Add(shortcut);
    }

}
