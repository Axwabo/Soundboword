using CommunityToolkit.Mvvm.ComponentModel;
using Soundboword.Models;

namespace Soundboword.Services;

public sealed partial class ShortcutAssigner : ObservableObject
{

    [ObservableProperty]
    public partial bool IsAssigning { get; set; }

    public ShortcutAction? Target { get; set; }

    public string? InputMethodFilter { get; set; }

    public void Close()
    {
        IsAssigning = false;
        Target = null;
        InputMethodFilter = null;
    }

}
