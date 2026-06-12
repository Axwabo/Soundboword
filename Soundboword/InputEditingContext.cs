using CommunityToolkit.Mvvm.ComponentModel;
using Soundboword.Models;
using Soundboword.Services;

namespace Soundboword;

public sealed partial class InputEditingContext : ObservableObject
{

    public InputEditingContext(ShortcutList list) => List = list;

    public ShortcutList List { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Name))]
    public partial InputMethodInterface? Interface { get; private set; }

    public string? Name => Interface?.Name;

    public void Open(InputMethodInterface method)
    {
        Close();
        Interface = method;
        List.Assigner.Target = StopAllSoundsAction.Instance;
        List.Assigner.InputMethodFilter = method.Name;
    }

    public void Close()
    {
        Interface = null;
        List.Assigner.Close();
    }

}
