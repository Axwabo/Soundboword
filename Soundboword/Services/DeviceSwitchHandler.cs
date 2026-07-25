namespace Soundboword.Services;

[RegisterSingleton(Registration = RegistrationStrategy.Self)]
public partial class DeviceSwitchHandler : ObservableObject
{

    [ObservableProperty]
    public partial bool IsSwitching { get; protected set; }

    public virtual Task OnOutputDeviceSwitchedAsync() => Task.CompletedTask;

    public virtual Task OnMicrophoneSwitchedAsync() => Task.CompletedTask;

}
