using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using Soundboword.Models;
using Soundboword.ViewModels;

namespace Soundboword;

public sealed partial class SoundEditingContext : ObservableObject
{

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Name), nameof(TriggerMode), nameof(Volume))]
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

    public TriggerMode TriggerMode
    {
        get => Model?.Mode ?? default;
        set
        {
            Model?.Mode = value;
            OnPropertyChanged();
        }
    }

    public float Volume
    {
        get => Model?.Volume ?? 1;
        set
        {
            Model?.Volume = value;
            OnPropertyChanged();
        }
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ButtonText))]
    [MemberNotNullWhen(true, nameof(Model))]
    public partial bool IsListeningForShortcuts { get; set; }

    public string ButtonText => IsListeningForShortcuts ? "Listening..." : "Add Shortcut";

    public void Open(SoundViewModel model)
    {
        Close();
        Model = model;
        Model.PropertyChanged += ModelOnPropertyChanged;
    }

    public void Close()
    {
        IsListeningForShortcuts = false;
        Model?.PropertyChanged -= ModelOnPropertyChanged;
        Model = null;
    }

    private void ModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e) => OnPropertyChanged(e.PropertyName);

    public void CancelShortcutAddition() => IsListeningForShortcuts = false;

}
