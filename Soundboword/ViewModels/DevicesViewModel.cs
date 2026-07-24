namespace Soundboword.ViewModels;

public sealed partial class DevicesViewModel : ViewModelBase
{

    private bool _isRefreshing;

    public DevicesViewModel() => DeviceManager = new SoundFlowDeviceManager();

    public DevicesViewModel(SoundFlowDeviceManager deviceManager)
    {
        DeviceManager = deviceManager;
        UpdateSelected();
    }

    public SoundFlowDeviceManager DeviceManager { get; }

    [ObservableProperty]
    public partial int SelectedDeviceIndex { get; set; }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (!_isRefreshing && e.PropertyName == nameof(SelectedDeviceIndex))
            DeviceManager.SwitchDevice(DeviceManager.Devices[SelectedDeviceIndex]);
    }

    [RelayCommand]
    private void Refresh() => Refresh(true);

    public bool Refresh(bool doNotSwitch)
    {
        _isRefreshing = doNotSwitch;
        DeviceManager.RefreshAudioDevices();
        var success = UpdateSelected();
        _isRefreshing = false;
        return success;
    }

    private bool UpdateSelected()
    {
        var deviceName = DeviceManager.SelectedDevice.Name;
        for (var i = 0; i < DeviceManager.Devices.Count; i++)
        {
            if (DeviceManager.Devices[i].Name != deviceName)
                continue;
            SelectedDeviceIndex = i;
            return true;
        }

        return false;
    }

}
