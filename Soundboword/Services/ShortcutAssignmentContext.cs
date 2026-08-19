using Soundboword.Inputs;

namespace Soundboword.Services;

[RegisterSingleton]
public sealed class ShortcutAssignmentContext
{

    public ShortcutAssignmentContext(ShortcutList list) => List = list;

    public ShortcutList List { get; }

    public ShortcutAssigner Assigner => List.Assigner;

    public IAssignmentKeyHandler? KeyHandler { get; set; }

}
