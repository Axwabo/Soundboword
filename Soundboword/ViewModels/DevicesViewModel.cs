namespace Soundboword.ViewModels;

public sealed partial class DevicesViewModel : ViewModelBase
{

    private bool _isRefreshing;

    public DevicesViewModel()
    {
        DeviceManager = new SoundFlowDeviceManager(new UserData());
        SwitchHandler = new DeviceSwitchHandler();
    }

    public DevicesViewModel(SoundFlowDeviceManager deviceManager, DeviceSwitchHandler handler)
    {
        DeviceManager = deviceManager;
        SwitchHandler = handler;
        deviceManager.OutputDeviceSwitched += () => _ = SwitchHandler.OnOutputDeviceSwitchedAsync();
        deviceManager.MicrophoneSwitched += () => _ = SwitchHandler.OnMicrophoneSwitchedAsync();
        UpdateSelected();
    }

    public SoundFlowDeviceManager DeviceManager { get; }

    public DeviceSwitchHandler SwitchHandler { get; }

    [ObservableProperty]
    public partial int SelectedDeviceIndex { get; set; }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (!_isRefreshing && e.PropertyName == nameof(SelectedDeviceIndex))
            SwitchToSelected();
    }

    public void SwitchToSelected() => DeviceManager.SwitchDevice(DeviceManager.Devices[SelectedDeviceIndex]);

    [RelayCommand]
    public void Refresh()
    {
        _isRefreshing = true;
        DeviceManager.RefreshAudioDevices();
        UpdateSelected();
        _isRefreshing = false;
    }

    private void UpdateSelected()
    {
        var deviceName = DeviceManager.SelectedDevice.Name;
        for (var i = 0; i < DeviceManager.Devices.Count; i++)
        {
            if (DeviceManager.Devices[i].Name != deviceName)
                continue;
            SelectedDeviceIndex = i;
            break;
        }
    }

}
