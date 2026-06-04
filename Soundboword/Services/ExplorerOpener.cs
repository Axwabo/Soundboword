using System.Diagnostics;

namespace Soundboword.Services;

public sealed class ExplorerOpener : IFileManagerOpener
{

    public void Open(string path) => Process.Start("explorer.exe", $"/select,\"{path}\"").Dispose();

}
