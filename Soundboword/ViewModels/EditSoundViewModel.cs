using System.ComponentModel;
using Avalonia.Input.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Soundboword.Models;
using Soundboword.Services;

namespace Soundboword.ViewModels;

public sealed partial class EditSoundViewModel : ViewModelBase
{

    private readonly HostControl? _host;
    private readonly IFileManagerOpener? _opener;

    public EditSoundViewModel()
    {
    }

    public EditSoundViewModel(HostControl host, IFileManagerOpener opener)
    {
        _host = host;
        _opener = opener;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Mode), nameof(Name), nameof(StopCommand))]
    public partial SoundViewModel? Model { get; private set; }

    public string Name
    {
        get => Model?.Name ?? "";
        set
        {
            Model?.Name = value;
            OnPropertyChanged();
        }
    }

    public PlaybackMode Mode
    {
        get => Model?.Mode ?? default;
        set
        {
            Model?.Mode = value;
            OnPropertyChanged();
        }
    }

    public IRelayCommand? StopCommand => Model?.StopCommand;

    public void Open(SoundViewModel model)
    {
        Close();
        Model = model;
        Model.PropertyChanged += ModelOnPropertyChanged;
    }

    public void Close()
    {
        Model?.PropertyChanged -= ModelOnPropertyChanged;
        Model = null;
    }

    private void ModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e) => OnPropertyChanged(e.PropertyName);

    [RelayCommand]
    private void TogglePause()
    {
        if (Model != null)
            AudioManager.TogglePause(Model);
    }

    [RelayCommand]
    private void CopyPath()
    {
        if (Model != null && _host is {Host.Clipboard: { } clipboard})
            clipboard.SetTextAsync(Model.Path);
    }

    [RelayCommand]
    private void Reveal()
    {
        if (Model != null)
            _opener?.Open(Model.Path);
    }

    [RelayCommand]
    private void Delete()
    {
        if (Model is not { } model)
            return;
        Close();
        model.List.Delete(model);
    }

}
