using System.Text;
using CliWrap;
using CliWrap.Buffered;

namespace Soundboword.Linux.PipeWire;

[RegisterSingleton]
public sealed class PipeWireCli
{

    private const string PwCli = "pw-cli";
    private const string PwLink = "pw-link";

    private static async Task<bool> DetectPipeWireAsync()
    {
        try
        {
            var stdout = new StringBuilder();
            var result = await Cli.Wrap(PwCli)
                .WithArguments("--version")
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdout))
                .WithStandardErrorPipe(PipeTarget.Null)
                .ExecuteBufferedAsync();
            return result.IsSuccess && stdout.ToString().StartsWith(PwCli);
        }
        catch
        {
            return false;
        }
    }

    public static async Task RestartAsync() => await Cli.Wrap("systemctl")
        .WithArguments(["--user", "restart", "pipewire", "pipewire-pulse", "wireplumber"])
        .ExecuteAsync();

    public static async Task<List<PipeWireObject>> ListObjectsAsync()
    {
        try
        {
            var result = await Cli.Wrap(PwCli)
                .WithArguments("ls")
                .ExecuteBufferedAsync();
            return PipeWireObjectReader.ReadObjectsAsync(result.StandardOutput);
        }
        catch
        {
            return [];
        }
    }

    public static async Task<IReadOnlyCollection<PipeWireLink>> ListLinksAsync()
    {
        try
        {
            var result = await Cli.Wrap(PwLink)
                .WithArguments("-lI")
                .ExecuteBufferedAsync();
            return PipeWireObjectReader.ReadLinksAsync(result.StandardOutput);
        }
        catch
        {
            return [];
        }
    }

    public static async Task<bool> LinkAsync(string sourcePort, string destinationPort, bool disconnect)
    {
        var result = await Cli.Wrap("pw-link")
            .WithArguments(disconnect ? ["-d", sourcePort, destinationPort] : [sourcePort, destinationPort])
            .WithValidation(CommandResultValidation.None)
            .ExecuteAsync()
            .ConfigureAwait(false);
        return result.IsSuccess;
    }

    public Task<bool> IsAvailable { get; } = DetectPipeWireAsync();

}
