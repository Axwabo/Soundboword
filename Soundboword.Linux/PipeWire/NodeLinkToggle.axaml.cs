namespace Soundboword.Linux.PipeWire;

public sealed partial class NodeLinkToggle : UserControl
{

    public static readonly StyledProperty<string?> TextProperty = AvaloniaProperty.Register<NodeLinkToggle, string?>(nameof(Text));

    public NodeLinkToggle() => InitializeComponent();

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TextProperty)
            Toggle.Text = change.NewValue;
    }

}
