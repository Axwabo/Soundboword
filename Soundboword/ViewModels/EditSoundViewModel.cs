using Avalonia.Input.Platform;
using Soundboword.Inputs;

namespace Soundboword.ViewModels;

public sealed partial class EditSoundViewModel : ViewModelBase
{

    private readonly AudioManager _audioManager;

    private readonly TopLevel? _topLevel;

    public EditSoundViewModel()
    {
        Context = new SoundEditingContext(new FilePicker());
        _audioManager = new AudioManager(new SoundFlowDeviceManager());
        Shortcuts = new ShortcutList(null, new ShortcutAssigner());
    }

    public EditSoundViewModel(TopLevel topLevel, IFileManagerOpener opener, SoundEditingContext context, AudioManager audioManager, ShortcutList shortcuts, IAssignmentKeyHandler? keyHandler = null)
    {
        _topLevel = topLevel;
        _audioManager = audioManager;
        Opener = opener;
        Shortcuts = shortcuts;
        Context = context;
        KeyHandler = keyHandler;
        Context.PropertyChanged += ContextOnPropertyChanged;
    }

    public IFileManagerOpener? Opener { get; }

    public ShortcutList Shortcuts { get; }

    public SoundEditingContext Context { get; }

    public IAssignmentKeyHandler? KeyHandler { get; }

    public ObservableCollection<Shortcut> Active => Shortcuts.Assigner.Active;

    [RelayCommand]
    private void Stop()
    {
        if (Context.Model != null)
            _audioManager.StopAll(Context.Model);
    }

    [RelayCommand]
    private async Task Relink()
    {
        if (Context.Model is { } model)
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
            Opener?.Open(path);
    }

    [RelayCommand]
    private void Delete()
    {
        if (Context.Model is not { } model)
            return;
        _audioManager.StopAll(model);
        Context.Close();
        model.List.Delete(model);
        Shortcuts.Remove(new TriggerSoundAction(model));
    }

    [RelayCommand]
    private void RemoveShortcuts()
    {
        if (Context.Model is not { } model)
            return;
        Active.Clear();
        Shortcuts.Remove(new TriggerSoundAction(model));
    }

    private void ContextOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SoundEditingContext.Model))
            UpdateActiveShortcuts();
    }

    private void UpdateActiveShortcuts()
    {
        if (Context.Model is not { } model)
            return;
        Active.Clear();
        foreach (var shortcut in Shortcuts.ForSound(model))
            Active.Add(shortcut);
    }

}
