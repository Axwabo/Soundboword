namespace Soundboword.Services;

[RegisterSingleton(Registration = RegistrationStrategy.Self)]
public partial class DeviceSwitchHandler : ObservableObject
{

    private Task? _task;

    [ObservableProperty]
    public partial bool IsSwitching { get; private set; }

    public Task PendingOperation => _task ?? Task.CompletedTask;

    public Task OnOutputDeviceSwitched() => Run(OnOutputDeviceSwitchedAsync);

    public void OnMicrophoneSwitched() => Run(OnMicrophoneSwitchedAsync);

    private Task Run(Func<Task> run)
    {
        if (_task is not {IsCompleted: false})
            _task = RunAsync(run);
        return _task;
    }

    private async Task RunAsync(Func<Task> run)
    {
        IsSwitching = true;
        try
        {
            await run();
        }
        finally
        {
            IsSwitching = false;
        }
    }

    protected virtual Task OnOutputDeviceSwitchedAsync() => Task.CompletedTask;

    protected virtual Task OnMicrophoneSwitchedAsync() => Task.CompletedTask;

}
