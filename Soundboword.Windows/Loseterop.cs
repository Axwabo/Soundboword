using System.Runtime.InteropServices;

namespace Soundboword.Windows;

// no i'm not naming it "winterop"
public static partial class Loseterop
{

    [LibraryImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

}
