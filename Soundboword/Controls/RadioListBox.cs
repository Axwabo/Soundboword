using Avalonia.Data;
using Avalonia.Metadata;

namespace Soundboword.Controls;

public sealed class RadioListBox : ListBox
{

    public static readonly StyledProperty<BindingBase?> ContentProperty = AvaloniaProperty.Register<RadioListBox, BindingBase?>(nameof(Content));

    [AssignBinding]
    [InheritDataTypeFromItems("ItemsSource")]
    public BindingBase? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

}
