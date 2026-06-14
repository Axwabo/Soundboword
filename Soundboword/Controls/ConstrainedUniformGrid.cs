using Avalonia.Controls.Primitives;

namespace Soundboword.Controls;

public sealed class ConstrainedUniformGrid : UniformGrid
{

    public static readonly StyledProperty<float> MaxItemWidthProperty = AvaloniaProperty.Register<ConstrainedUniformGrid, float>(nameof(MaxItemWidth), 280);

    public float MaxItemWidth
    {
        get => GetValue(MaxItemWidthProperty);
        set => SetValue(MaxItemWidthProperty, value);
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        if (e.WidthChanged)
            Columns = (int) Math.Ceiling(e.NewSize.Width / MaxItemWidth);
    }

}
