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

    private static async Task WaitForRestartAsync(SoundFlowDeviceManager manager)
    {
        var count = manager.Devices.Count;
        for (var i = 0; i < 10; i++)
        {
            await Task.Delay(500);
            manager.RefreshAudioDevices();
            manager.RefreshMidiInputs();
            if (manager.Devices.Count != count)
                break;
        }
    }

    private static void DisableInputs(InputsViewModel inputs, HashSet<InputMethodInterface> disabled)
    {
        foreach (var method in inputs.Available)
        {
            if (!method.Activated)
                continue;
            method.Activated = false;
            disabled.Add(method);
        }
    }

    private static string Config => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    private readonly RestartContext? _context;

    private readonly PipeWireWizardWindow? _window;

    public PipeWireWizardWindowViewModel() : this(null, null)
    {
    }

    public PipeWireWizardWindowViewModel(PipeWireWizardWindow? window, RestartContext? context)
    {
        _window = window;
        _context = context;
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        TargetDirectory = Path.Combine(Config.Replace(home, "~"), Directories);
    }

    public string TargetDirectory { get; }

    [RelayCommand]
    private async Task Run()
    {
        if (_window == null || _context is not var (audio, devices, inputs, nodeManager))
            return;
        var restartAttempted = false;
        var disabled = new HashSet<InputMethodInterface>();
        try
        {
            audio.StopAll();
            var directory = Path.Combine(Config, Directories);
            Directory.CreateDirectory(directory);
            await WriteFileAsync(directory);
            restartAttempted = true;
            DisableInputs(inputs, disabled);
            devices.DeviceManager.Dispose();
            await PipeWireCli.RestartAsync();
            _window.Close();
        }
        catch (Exception e)
        {
            var error = e is {InnerException.Message: var message}
                ? $"{e.Message}\n\n{message}"
                : e.Message;
            await ErrorDialogWindow.ShowAsync(error, _window);
        }

        if (!restartAttempted)
            return;
        devices.DeviceManager.InitializeEngine();
        await WaitForRestartAsync(devices.DeviceManager);
        await nodeManager.Refresh();
        devices.Refresh();
        if (devices.DeviceManager.Devices.Count == 0)
            devices.DeviceManager.SwitchToDefaultDevice();
        else
            devices.SwitchToSelected();
        inputs.Refresh();
        foreach (var method in inputs.Available)
            if (disabled.Contains(method))
                method.Activated = true;
    }

}
