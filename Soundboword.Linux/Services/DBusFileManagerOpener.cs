using CliWrap;

namespace Soundboword.Linux.Services;

[RegisterSingleton<IFileManagerOpener>]
public sealed class DBusFileManagerOpener : IFileManagerOpener
{

    // doesn't work without print-reply
    private const string Format = "--print-reply --dest=org.freedesktop.FileManager1 /org/freedesktop/FileManager1 org.freedesktop.FileManager1.ShowItems array:string:\"file://{0}\" string:\"\"";

    public void Open(string path)
    {
        using var wrap = Cli.Wrap("dbus-send")
            .WithArguments(string.Format(Format, path))
            .WithStandardOutputPipe(PipeTarget.Null)
            .ExecuteAsync();
        wrap.Task.Wait();
    }

}
