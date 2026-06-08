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

}
