using System.Threading;
using Avalonia.Interactivity;

namespace Soundboword.Views;

public sealed partial class SoundPlaybackView : UserControl
{

    public static readonly StyledProperty<AudioManager> ManagerProperty = AvaloniaProperty.Register<SoundPlaybackView, AudioManager>(nameof(Manager));

    private CancellationTokenSource? _cts;

    public SoundPlaybackView() => InitializeComponent();

    public AudioManager Manager
    {
        get => GetValue(ManagerProperty);
        set => SetValue(ManagerProperty, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        Update();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;
        _ = Task.Run(() => UpdateAsync(token), token);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is not SoundPlayback playback)
            return;
        SoundName.Text = playback.Name;
        Progress.Maximum = playback.Player.Duration;
    }

    private async Task UpdateAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(100));
        while (await timer.WaitForNextTickAsync(cancellationToken))
            Dispatcher.Post(Update);
    }

    private void Update()
    {
        if (IsEffectivelyVisible && DataContext is SoundPlayback playback)
            Progress.Value = playback.Player.Time;
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SoundPlayback playback)
            Manager.Stop(playback);
    }

}
