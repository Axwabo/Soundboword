namespace Soundboword.Linux.PipeWire.Wizard;

public sealed partial class PipeWireWizardWindowViewModel : ViewModelBase
{

    public const string FileName = "Soundboword.conf";

    private static readonly string Directories = Path.Combine("pipewire", "pipewire.conf.d");

    private static async Task WriteFileAsync(string directory)
    {
        var assembly = typeof(PipeWireWizardWindowViewModel).Assembly;
        foreach (var resource in assembly.GetManifestResourceNames())
        {
            if (!resource.Contains(FileName))
                continue;
            await using var resourceStream = assembly.GetManifestResourceStream(resource);
            if (resourceStream == null)
                continue;
            await using var file = File.Create(Path.Combine(directory, FileName));
            await resourceStream.CopyToAsync(file);
            return;
        }

        throw new FileNotFoundException("Could not find the configuration template");
    }

    private static async Task RunAsync()
    {
        var directory = Path.Combine(Config, Directories);
        Directory.CreateDirectory(directory);
        await WriteFileAsync(directory);
        await PipeWireCli.RestartAsync();
    }

    private static string Config => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    private readonly AudioManager? _audioManager;
    private readonly DevicesViewModel? _devices;

    private readonly PipeWireWizardWindow? _window;

    public PipeWireWizardWindowViewModel() : this(null, null, null)
    {
    }

    public PipeWireWizardWindowViewModel(PipeWireWizardWindow? window, AudioManager? audioManager, DevicesViewModel? devices)
    {
        _window = window;
        _audioManager = audioManager;
        _devices = devices;
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        TargetDirectory = Path.Combine(Config.Replace(home, "~"), Directories);
    }

    public string TargetDirectory { get; }

    [RelayCommand]
    private async Task Run()
    {
        if (_window == null || _audioManager == null || _devices == null)
            return;
        try
        {
            _audioManager.StopAll();
            _devices.DeviceManager.Dispose();
            await RunAsync();
            _window.Close();
        }
        catch (Exception e)
        {
            var error = e is {InnerException.Message: var message}
                ? $"{e.Message}\n\n{message}"
                : e.Message;
            await ErrorDialogWindow.ShowAsync(error, _window);
        }
        finally
        {
            _devices.DeviceManager.InitializeEngine();
            if (!_devices.Refresh(false))
                _devices.DeviceManager.SwitchToDefaultDevice();
        }
    }

}
