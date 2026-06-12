using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Soundboword.Services;

namespace Soundboword.ViewModels;

public sealed partial class DevicesViewModel : ViewModelBase
{

    public SoundFlowDeviceManager DeviceManager { get; }

    public DevicesViewModel() => DeviceManager = new SoundFlowDeviceManager();

    public DevicesViewModel(SoundFlowDeviceManager deviceManager)
    {
        DeviceManager = deviceManager;
        SelectedDeviceIndex = deviceManager.Devices.IndexOf(deviceManager.SelectedDevice);
    }

    [ObservableProperty]
    public partial int SelectedDeviceIndex { get; set; }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(SelectedDeviceIndex))
            DeviceManager.SwitchDevice(DeviceManager.Devices[SelectedDeviceIndex]);
    }

}
