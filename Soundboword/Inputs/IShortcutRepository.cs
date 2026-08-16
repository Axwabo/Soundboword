namespace Soundboword.Inputs;

public interface IShortcutRepository
{

    public const string Key = "Shortcuts";

    string InputMethodName { get; }

    IEnumerable<Shortcut> All { get; }

    IEnumerable<Shortcut> GetAll(string actionId);

    void RemoveAll(ShortcutAction action);

    void Commit();

}
