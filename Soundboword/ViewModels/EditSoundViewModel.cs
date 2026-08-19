using Avalonia.Input.Platform;

namespace Soundboword.ViewModels;

public sealed partial class EditSoundViewModel : ViewModelBase
{

    private readonly AudioManager _audioManager;

    private readonly TopLevel? _topLevel;

    public EditSoundViewModel()
    {
        SoundContext = new SoundEditingContext(new FilePicker());
        _audioManager = new AudioManager(new SoundFlowDeviceManager());
        InputContext = new InputEditingContext(new ShortcutList(null, new ShortcutAssigner(), []));
    }

    public EditSoundViewModel(TopLevel topLevel, IFileManagerOpener opener, SoundEditingContext soundContext, AudioManager audioManager, InputEditingContext inputContext)
    {
        _topLevel = topLevel;
        _audioManager = audioManager;
        Opener = opener;
        SoundContext = soundContext;
        InputContext = inputContext;
        SoundContext.PropertyChanged += SoundContextOnPropertyChanged;
    }

    public IFileManagerOpener? Opener { get; }

    public ShortcutList Shortcuts => InputContext.List;

    public SoundEditingContext SoundContext { get; }

    public InputEditingContext InputContext { get; }

    public ObservableCollection<Shortcut> Active => InputContext.Assigner.Active;

    [RelayCommand]
    private void Stop()
    {
        if (SoundContext.Model != null)
            _audioManager.StopAll(SoundContext.Model);
    }

    [RelayCommand]
    private async Task Relink()
    {
        if (SoundContext.Model is { } model)
            await model.Relink();
    }

    [RelayCommand]
    private void OpenEditor()
    {
        // TODO
    }

    [RelayCommand]
    private void TogglePause()
    {
        if (SoundContext.Model != null)
            _audioManager.TogglePause(SoundContext.Model);
    }

    [RelayCommand]
    private void CopyPath()
    {
        if (SoundContext.Model is {Path: var path} && _topLevel is {Clipboard: { } clipboard})
            clipboard.SetTextAsync(path);
    }

    [RelayCommand]
    private void Reveal()
    {
        if (SoundContext.Model is {Path: var path})
            Opener?.Open(path);
    }

    [RelayCommand]
    private void Delete()
    {
        if (SoundContext.Model is not { } model)
            return;
        _audioManager.StopAll(model);
        SoundContext.Close();
        model.List.Delete(model);
        Shortcuts.Remove(new TriggerSoundAction(model));
    }

    [RelayCommand]
    private void RemoveShortcuts()
    {
        if (SoundContext.Model is not { } model)
            return;
        Active.Clear();
        Shortcuts.Remove(new TriggerSoundAction(model));
    }

    private void SoundContextOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SoundEditingContext.Model))
            UpdateActiveShortcuts();
    }

    private void UpdateActiveShortcuts()
    {
        if (SoundContext.Model is not { } model)
            return;
        Active.Clear();
        foreach (var shortcut in Shortcuts.ForSound(model))
            Active.Add(shortcut);
    }

}
