namespace Soundboword.Controls;

public sealed partial class LazyControl : UserControl
{

    public static readonly StyledProperty<bool> IsLazyProperty = AvaloniaProperty.Register<LazyControl, bool>(nameof(IsLazy));

    public static readonly StyledProperty<bool> IsActiveProperty = AvaloniaProperty.Register<LazyControl, bool>(nameof(IsActive));

    public static readonly StyledProperty<object?> ViewModelProperty = AvaloniaProperty.Register<LazyControl, object?>(nameof(ViewModel));

    public LazyControl() => InitializeComponent();

    public bool IsLazy
    {
        get => GetValue(IsLazyProperty);
        set => SetValue(IsLazyProperty, value);
    }

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public object? ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsLazyProperty)
        {
        }

        if (change.Property == ViewModelProperty)
        {
            if (!IsLazy || IsActive)
                ContentControl.Content = ViewModel;
        }

        if (change.Property == IsActiveProperty)
        {
            // TODO
        }
    }

}
