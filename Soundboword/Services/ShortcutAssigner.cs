using CommunityToolkit.Mvvm.ComponentModel;
using Soundboword.Models;

namespace Soundboword.Services;

public sealed partial class ShortcutAssigner : ObservableObject
{

    [ObservableProperty]
    public partial bool IsAssigning { get; set; }

    public ShortcutAction? Target { get; set; }

    public void Cancel()
    {
        IsAssigning = false;
        Target = null;
    }

}
