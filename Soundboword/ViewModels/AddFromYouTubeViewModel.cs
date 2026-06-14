using System.Threading;

namespace Soundboword.ViewModels;

public sealed partial class AddFromYouTubeViewModel : ViewModelBase
{

    private CancellationTokenSource? _cts = new();

    [RelayCommand]
    public void Cancel()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

}
