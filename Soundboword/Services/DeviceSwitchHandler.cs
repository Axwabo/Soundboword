namespace Soundboword.Services;

[RegisterSingleton(Registration = RegistrationStrategy.Self)]
public partial class DeviceSwitchHandler : ObservableObject
{

    [ObservableProperty]
    public partial bool IsSwitching { get; private set; }

    public void OnOutputDeviceSwitched()
    {
        if (!IsSwitching)
            _ = Run(OnOutputDeviceSwitchedAsync);
    }

    public void OnMicrophoneSwitched()
    {
        if (!IsSwitching)
            _ = Run(OnMicrophoneSwitchedAsync);
    }

    private async Task Run(Func<Task> run)
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
