using Avalonia.Interactivity;

namespace Soundboword.Views;

public sealed partial class SoundPlaybackView : UserControl
{

    public static readonly StyledProperty<AudioManager> ManagerProperty = AvaloniaProperty.Register<SoundPlaybackView, AudioManager>(nameof(Manager));

    public SoundPlaybackView() => InitializeComponent();

    public AudioManager Manager
    {
        get => GetValue(ManagerProperty);
        set => SetValue(ManagerProperty, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) => RequestUpdate();

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is not SoundPlayback playback)
            return;
        SoundName.Text = playback.Name;
        Progress.Maximum = playback.Player.Duration;
    }

    private void Update()
    {
        if (!IsEffectivelyVisible || DataContext is not SoundPlayback playback)
            return;
        Progress.Value = playback.Player.Time;
        RequestUpdate();
    }

    private void RequestUpdate() => TopLevel.GetTopLevel(this)?.RequestAnimationFrame(_ => Update());

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SoundPlayback playback)
            Manager.Stop(playback);
    }

}
