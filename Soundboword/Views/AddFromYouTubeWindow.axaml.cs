namespace Soundboword.Views;

public sealed partial class AddFromYouTubeWindow : Window
{

    private readonly AddFromYouTubeViewModel _model;

    private readonly IServiceScope _scope;

    private AddFromYouTubeWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _scope = serviceProvider.CreateScope();
        _model = _scope.ServiceProvider.GetRequiredService<AddFromYouTubeViewModel>();
        _model.Canceled += Close;
        DataContext = _model;
    }

    public static void Show(IServiceProvider serviceProvider) => new AddFromYouTubeWindow(serviceProvider).Show();

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);
        _model.Cancel();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        _scope.Dispose();
    }

}
