namespace Soundboword.Controls;

public sealed class DataToggle : ToggleSwitch
{

    public static readonly StyledProperty<object?> TextProperty = AvaloniaProperty.Register<DataToggle, object?>(nameof(Text));

    public object? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    protected override Type StyleKeyOverride => typeof(ToggleSwitch);

    protected override void Toggle()
    {
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property != TextProperty)
            return;
        OnContent = change.NewValue;
        OffContent = change.NewValue;
    }

}
