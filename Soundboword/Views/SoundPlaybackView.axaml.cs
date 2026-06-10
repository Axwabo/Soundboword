using System;
using Avalonia.Controls;
using Soundboword.Models;

namespace Soundboword.Views;

public sealed partial class SoundPlaybackView : UserControl
{

    public SoundPlaybackView()
    {
        InitializeComponent();
        Dispatcher.Post(Update); // TODO: optimize
    }

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
        Dispatcher.Post(Update);
        if (IsEffectivelyVisible && DataContext is SoundPlayback playback)
            Progress.Value = playback.Player.Time;
    }

}
