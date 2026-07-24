namespace Soundboword.Controls;

public sealed class DataToggle : ToggleSwitch
{

    public static readonly StyledProperty<object?> TextProperty = AvaloniaProperty.Register<DataToggle, object?>(nameof(Text));

    public object? Text
    {
        get => GetValue(TextProperty);
        set
        {
            SetValue(TextProperty, value);
            OnContent = value;
            OffContent = value;
        }
    }

    protected override Type StyleKeyOverride => typeof(ToggleSwitch);

    protected override void Toggle()
    {
    }

}
