namespace Soundboword.Inputs;

public interface IShortcutRepository
{

    ShortcutList? List { set; }

    string InputMethodName { get; }

    IEnumerable<Shortcut> All { get; }

    IEnumerable<Shortcut> GetAll(ShortcutAction action);

    void RemoveAll(ShortcutAction action);

    void Commit();

}
