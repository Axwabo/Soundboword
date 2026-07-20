using Avalonia.Input;
using Avalonia.Platform.Storage;

// ReSharper disable UnusedParameter.Local

// ReSharper disable UnusedMember.Local

namespace Soundboword.Views;

public sealed partial class BoardView : UserControl
{

    private static bool AllowDrop(DragEventArgs e) => e.Source is Panel presenter && DragDrop.GetAllowDrop(presenter);

    private static IEnumerable<string> MapPaths(IStorageItem[] items)
    {
        foreach (var item in items)
        {
            var path = item.TryGetLocalPath();
            if (!string.IsNullOrEmpty(path) && (path.EndsWith(".mp3") || path.EndsWith(".wav")))
                yield return path;
        }
    }

    public BoardView() => InitializeComponent();

    private BoardViewModel? Model => DataContext as BoardViewModel;

    private void InputElement_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        => Model?.Editor.Context.Close();

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        var contains = e.DataTransfer.Formats.Contains(DataFormat.File);
        e.DragEffects = contains ? DragDropEffects.Copy : DragDropEffects.None;
        Model?.DragOver = contains;
    }

    private void OnDragLeave(object? sender, DragEventArgs e)
    {
        if (AllowDrop(e))
            Model?.DragOver = false;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        Model?.DragOver = false;
        if (!e.DataTransfer.Formats.Contains(DataFormat.File))
            return;
        var files = e.DataTransfer.TryGetFiles();
        if (files != null)
            Model?.List.Add(MapPaths(files));
    }

}
