using System.Diagnostics;
using Soundboword.Services;

namespace Soundboword.Windows.Services;

public sealed class ExplorerOpener : IFileManagerOpener
{

    public void Open(string path) => Process.Start("explorer.exe", $"/select,\"{path}\"").Dispose();

}
