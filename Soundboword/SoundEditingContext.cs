using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Soundboword.Models;
using Soundboword.ViewModels;

namespace Soundboword;

public sealed partial class SoundEditingContext : ObservableObject
{

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PlaybackMode), nameof(Name))]
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

    public PlaybackMode PlaybackMode
    {
        get => Model?.Mode ?? default;
        set
        {
            Model?.Mode = value;
            OnPropertyChanged();
        }
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ButtonText))]
    public partial bool IsListeningForShortcuts { get; set; }

    public SoundViewModel? Listening { get; private set; }

    public string ButtonText => IsListeningForShortcuts ? "Listening..." : "Add Shortcut";

    public void Open(SoundViewModel model)
    {
        Close();
        Model = model;
        Model.PropertyChanged += ModelOnPropertyChanged;
    }

    public void Close()
    {
        Listening = null;
        Model?.PropertyChanged -= ModelOnPropertyChanged;
        Model = null;
    }

    private void ModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e) => OnPropertyChanged(e.PropertyName);

    public void ToggleListening()
    {
        if (Model is not { } model)
            return;
        if (IsListeningForShortcuts)
            Listening = model;
        else
            CancelShortcutAddition();
    }

    public void CancelShortcutAddition()
    {
        Listening = null;
        IsListeningForShortcuts = false;
    }

}
