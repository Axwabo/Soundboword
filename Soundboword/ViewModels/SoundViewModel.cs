using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Soundboword.ViewModels;

public partial class SoundViewModel : ViewModelBase
{

    [ObservableProperty]
    public required partial string Name { get; set; }

    public required string Path { get; init; }

    [RelayCommand]
    private void Play() => AudioManager.Play(this);

}
