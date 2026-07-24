using System.Text;
using CliWrap;
using CliWrap.Buffered;

namespace Soundboword.Linux.PipeWire;

[RegisterSingleton]
public sealed class PipeWireCli
{

    private static async Task<bool> DetectPipeWireAsync()
    {
        try
        {
            var stdout = new StringBuilder();
            var result = await Cli.Wrap("pw-cli")
                .WithArguments("--version")
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdout))
                .WithStandardErrorPipe(PipeTarget.Null)
                .ExecuteBufferedAsync();
            return result.IsSuccess && stdout.ToString().StartsWith("pw-cli");
        }
        catch
        {
            return false;
        }
    }

    public static async Task RestartAsync() => await Cli.Wrap("systemctla")
        .WithArguments(["--user", "restart", "pipewire", "pipewire-pulse", "wireplumber"])
        .ExecuteAsync();

    public Task<bool> IsAvailable { get; } = DetectPipeWireAsync();

}
