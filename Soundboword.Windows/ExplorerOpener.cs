using System.Diagnostics;

namespace Soundboword.Windows;

public sealed class ExplorerOpener : IFileManagerOpener
{

    public void Open(string path) => Process.Start("explorer.exe", $"/select,\"{path}\"").Dispose();

}
