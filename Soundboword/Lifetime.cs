using Avalonia.Controls.ApplicationLifetimes;

namespace Soundboword;

[RegisterSingleton]
public sealed class Lifetime
{

    private readonly List<(ShutdownPriority Priority, Action Action)> _callbacks = [];

    public Lifetime(IClassicDesktopStyleApplicationLifetime? lifetime = null)
    {
        IsActive = lifetime != null;
        lifetime?.Exit += (_, _) => ShutdownServices();
    }

    public bool IsActive { get; }

    public event Action Exit
    {
        add => Register(value);
        remove => throw new NotSupportedException();
    }

    private void ShutdownServices()
    {
        _callbacks.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        foreach (var (_, action) in _callbacks)
            action();
    }

    public void Register(Action callback, ShutdownPriority priority = ShutdownPriority.Normal)
        => _callbacks.Add((priority, callback));

}

public enum ShutdownPriority
{

    Normal = 0,
    Final = int.MaxValue

}
