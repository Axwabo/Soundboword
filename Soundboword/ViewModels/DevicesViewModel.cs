namespace Soundboword.ViewModels;

public sealed partial class DevicesViewModel : ViewModelBase
{

    private bool _isRefreshing;

    public DevicesViewModel() => DeviceManager = new SoundFlowDeviceManager();

    public DevicesViewModel(SoundFlowDeviceManager deviceManager)
    {
        DeviceManager = deviceManager;
        deviceManager.DeviceSwitched += () => _ = DeviceManagerOnDeviceSwitched();
        UpdateSelected();
    }

    public SoundFlowDeviceManager DeviceManager { get; }

    [ObservableProperty]
    public partial int SelectedDeviceIndex { get; set; }

    [ObservableProperty]
    public partial bool IsSwitching { get; private set; }

    public Func<Task>? DeviceSwitched { get; set; }

    private async Task DeviceManagerOnDeviceSwitched()
    {
        if (DeviceSwitched == null)
            return;
        IsSwitching = true;
        try
        {
            await DeviceSwitched();
        }
        finally
        {
            IsSwitching = false;
        }
    }

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
