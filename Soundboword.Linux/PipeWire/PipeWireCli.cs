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

    public Task<bool> IsAvailable { get; } = DetectPipeWireAsync();

}
