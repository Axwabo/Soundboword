using System.Runtime.InteropServices;

namespace Soundboword.Windows;

// no i'm not naming it "winterop"
public static partial class Loseterop
{

    public const int HotkeyMessageId = 0x0312;

    [LibraryImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [LibraryImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool UnregisterHotKey(IntPtr hWnd, int id);

}
