using System.Diagnostics;

namespace Soundboword.Linux.Services;

public sealed class DBusFileManagerOpener : IFileManagerOpener
{

    // doesn't work without print-reply
    private const string Format = "--print-reply --dest=org.freedesktop.FileManager1 /org/freedesktop/FileManager1 org.freedesktop.FileManager1.ShowItems array:string:\"file://{0}\" string:\"\"";

    public void Open(string path)
    {
        using var process = Process.Start(new ProcessStartInfo("dbus-send", string.Format(Format, path)) {RedirectStandardOutput = true});
    }

}
