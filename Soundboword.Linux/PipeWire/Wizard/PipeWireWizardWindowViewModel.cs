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

    private readonly PipeWireWizardWindow? _window;

    public PipeWireWizardWindowViewModel() : this(null)
    {
    }

    public PipeWireWizardWindowViewModel(PipeWireWizardWindow? window)
    {
        _window = window;
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        TargetDirectory = Path.Combine(Config.Replace(home, "~"), Directories);
    }

    public string TargetDirectory { get; }

    [ObservableProperty]
    public partial string? Error { get; private set; }

    [RelayCommand]
    private async Task Run()
    {
        if (_window == null)
            return;
        Error = "";
        try
        {
            await RunAsync();
            _window.Close();
        }
        catch (Exception e)
        {
            Error = e is {InnerException.Message: var message}
                ? $"{e.Message}\n\n{message}"
                : e.Message;
        }
    }

}
