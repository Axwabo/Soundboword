namespace Soundboword.Controls;

public sealed partial class LazyControl : UserControl
{

    public static readonly StyledProperty<bool> IsLazyProperty = AvaloniaProperty.Register<LazyControl, bool>(nameof(IsLazy));

    public static readonly StyledProperty<object?> ViewModelProperty = AvaloniaProperty.Register<LazyControl, object?>(nameof(ViewModel));

    public LazyControl() => InitializeComponent();

    public bool IsLazy
    {
        get => GetValue(IsLazyProperty);
        set => SetValue(IsLazyProperty, value);
    }

    public object? ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsVisibleProperty)
        {
        }
    }

}
