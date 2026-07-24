using System.Text;
using CliWrap;
using CliWrap.Buffered;

namespace Soundboword.Linux.PipeWire;

[RegisterSingleton]
public sealed class PipeWireCli
{

    private const string PwCli = "pw-cli";

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

    public static async Task<List<PipeWireNode>> ListNodesAsync()
    {
        try
        {
            var result = await Cli.Wrap(PwCli)
                .WithArguments("ls")
                .ExecuteBufferedAsync();
            using var reader = new StringReader(result.StandardOutput);
            return PipeWireNodeReader.ReadAudioNodesAsync(reader);
        }
        catch
        {
            return [];
        }
    }

    public Task<bool> IsAvailable { get; } = DetectPipeWireAsync();

}
