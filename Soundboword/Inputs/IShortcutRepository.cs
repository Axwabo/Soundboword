namespace Soundboword.Inputs;

public interface IShortcutRepository
{

    public const string Key = "Shortcuts";

    string InputMethodName { get; }

    IEnumerable<Shortcut> All { get; }

    IEnumerable<Shortcut> GetAll(ShortcutAction action);

    void RemoveAll(ShortcutAction action);

    void Commit();

}
