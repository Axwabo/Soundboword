namespace Soundboword.Logging;

public sealed partial class LogPreferencesView : UserControl
{

    public LogPreferencesView() => InitializeComponent();

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is LogPreferences preferences)
            preferences.PropertyChanged += PreferencesOnPropertyChanged;
    }

    private void PreferencesOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(LogPreferences.HideBottomBar))
            Note.IsVisible = true;
    }

}
