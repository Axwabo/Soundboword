using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Soundboword.Views;

public partial class MainWindow : Window
{

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        AudioManager.Init();
    }

    private void TopLevel_OnClosed(object? sender, EventArgs e)
    {
        AudioManager.Destroy();
    }

}
