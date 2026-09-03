namespace Soundboword.Controls;

public abstract class DisposableWindow : Window
{

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        (DataContext as IDisposable)?.Dispose();
    }

}
