using CommunityToolkit.Mvvm.ComponentModel;

namespace Soundboword.ViewModels;

public partial class SoundViewModel : ViewModelBase
{

    [ObservableProperty]
    public required partial string Name { get; set; }

    public required string Path { get; init; }

}
