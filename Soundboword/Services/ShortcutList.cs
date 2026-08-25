using Soundboword.Inputs;

namespace Soundboword.Services;

[RegisterSingleton]
public sealed class ShortcutList
{

    internal static void NotifyShortcutsChanged() => ShortcutsChanged?.Invoke();

    private readonly HashSet<Shortcut> _all = [];

    private readonly List<IShortcutRepository> _repositories;

    public ShortcutList(Lifetime lifetime, ShortcutAssigner assigner, IEnumerable<IShortcutRepository> repositories, IAssignmentKeyHandler? keyHandler = null)
    {
        Assigner = assigner;
        KeyHandler = keyHandler;
        _repositories = repositories.ToList();
        foreach (var repository in _repositories)
            _all.UnionWith(repository.All);
        lifetime.Exit += () =>
        {
            foreach (var repository in _repositories)
                repository.Commit();
        };
    }

    public ShortcutAssigner Assigner { get; }

    public IAssignmentKeyHandler? KeyHandler { get; }

    public static event Action? ShortcutsChanged;

    public IEnumerable<Shortcut> For(string inputMethod, ShortcutAction action)
    {
        foreach (var repository in _repositories)
            if (repository.InputMethodName == inputMethod)
                foreach (var shortcut in repository.GetAll(action))
                    yield return shortcut;
    }

    public IEnumerable<Shortcut> ForSound(SoundViewModel sound)
    {
        foreach (var repository in _repositories)
        foreach (var shortcut in repository.GetAll(new TriggerSoundAction(sound)))
            yield return shortcut;
    }

    public IEnumerable<Shortcut> ForRepository(string name)
    {
        foreach (var repository in _repositories)
            if (repository.InputMethodName == name)
                return repository.All;
        return [];
    }

    public void Trigger<T>(T key, string inputMethod) where T : notnull
    {
        if (!Assigner.IsAssigning)
        {
            foreach (var repository in _repositories)
                if (repository is ShortcutRepository<T> implementation)
                    implementation.Trigger(key);
            return;
        }

        if (Assigner.InputMethodFilter is { } filter && filter != inputMethod)
            return;
        if (Assigner.Target is { } action)
            Assign(key, action);
        Assigner.IsAssigning = false;
    }

    public void Assign<T>(T key, ShortcutAction action) where T : notnull
    {
        var changed = false;
        foreach (var repository in _repositories)
        {
            if (repository is not ShortcutRepository<T> implementation || implementation.Assign(key, action, _all) is not { } shortcut)
                continue;
            changed = true;
            var index = Assigner.Active.FindInputMethodIndex(repository.InputMethodName);
            if (index == -1)
                Assigner.Active.Add(shortcut);
            else
                Assigner.Active[index] = shortcut;
        }

        if (changed)
            NotifyShortcutsChanged();
    }

    public void Remove(ShortcutAction action)
    {
        if (Assigner.Target == action)
            Assigner.IsAssigning = false;
        foreach (var repository in _repositories)
            repository.RemoveAll(action);
        var removed = _all.RemoveWhere(e => e.Action == action);
        if (removed == 0)
            return;
        Assigner.Active.Clear();
        NotifyShortcutsChanged();
    }

    public T RequireRepository<T>() where T : IShortcutRepository => _repositories.OfType<T>().First();

}
