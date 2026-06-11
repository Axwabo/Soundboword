using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Soundboword.Models;
using Soundboword.Services;
using Soundboword.ViewModels;

namespace Soundboword;

public sealed partial class SoundEditingContext : ObservableObject
{

    private readonly ShortcutAssigner? _assigner;

    public SoundEditingContext(ShortcutAssigner? assigner = null) => _assigner = assigner;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Name), nameof(TriggerMode), nameof(Volume), nameof(CanRelink))]
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

    public bool CanRelink => Model?.PlaybackState is SoundState.Stopped or SoundState.Error;

    public void Open(SoundViewModel model)
    {
        Close();
        _assigner?.Target = new TriggerSoundAction(model);
        Model = model;
        Model.PropertyChanged += ModelOnPropertyChanged;
    }

    public void Close()
    {
        _assigner?.Close();
        Model?.PropertyChanged -= ModelOnPropertyChanged;
        Model = null;
    }

    private void ModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(e.PropertyName);
        if (e.PropertyName == nameof(SoundViewModel.PlaybackState))
            OnPropertyChanged(nameof(CanRelink));
    }

}
