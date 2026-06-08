using Avalonia.Input.Platform;
using CommunityToolkit.Mvvm.Input;
using Soundboword.Services;

namespace Soundboword.ViewModels;

public sealed partial class EditSoundViewModel : ViewModelBase
{

    private readonly HostControl? _host;
    private readonly IFileManagerOpener? _opener;
    private readonly InputsViewModel _inputs;

    public SoundEditingContext Context { get; }

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
    }

    [RelayCommand]
    private void ToggleListening() => Context.ToggleListening(_inputs);

    [RelayCommand]
    private void RemoveShortcuts()
    {
        if (Context.Model is not { } model)
            return;
        foreach (var input in _inputs.Available)
            input.ClearShortcut(model);
    }

}
