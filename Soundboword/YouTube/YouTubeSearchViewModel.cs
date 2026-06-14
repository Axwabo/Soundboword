namespace Soundboword.YouTube;

public sealed partial class YouTubeSearchViewModel : ViewModelBase
{

    [ObservableProperty]
    public partial string Query { get; set; } = "";

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
    }

}
