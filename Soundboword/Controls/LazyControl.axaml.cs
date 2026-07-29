using Avalonia.Interactivity;
using Page = Soundboword.ViewModels.Page;

namespace Soundboword.Controls;

public sealed partial class LazyControl : UserControl
{

    public static readonly StyledProperty<bool> IsLazyProperty = AvaloniaProperty.Register<LazyControl, bool>(nameof(IsLazy));

    public static readonly StyledProperty<object?> ActiveObjectProperty = AvaloniaProperty.Register<LazyControl, object?>(nameof(ActiveObject));

    public static readonly StyledProperty<Page?> PageProperty = AvaloniaProperty.Register<LazyControl, Page?>(nameof(Page));

    public LazyControl() => InitializeComponent();

    public bool IsLazy
    {
        get => GetValue(IsLazyProperty);
        set => SetValue(IsLazyProperty, value);
    }

    public object? ActiveObject
    {
        get => GetValue(ActiveObjectProperty);
        set => SetValue(ActiveObjectProperty, value);
    }

    public Page? Page
    {
        get => GetValue(PageProperty);
        set => SetValue(PageProperty, value);
    }

    private bool IsActive => ActiveObject == Page;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (!IsLoaded || change.Property != ActiveObjectProperty)
            return;
        if (IsLazy)
        {
            InnerControl.Content = IsActive ? Page?.Content : null;
            return;
        }

        InnerControl.Content ??= Page?.Content;
        InnerControl.IsVisible = IsActive;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (IsLazy && !IsActive)
            return;
        InnerControl.Content = Page?.Content;
        InnerControl.IsVisible = IsActive;
    }

}
