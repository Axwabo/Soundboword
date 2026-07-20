using Soundboword.Generated;
using Tmds.DBus.Protocol;

namespace Soundboword.Linux;

internal static class DestkopPortal
{

    public const string Bus = "org.freedesktop.portal.Desktop";

    public const string Path = "/org/freedesktop/portal/desktop";

    public static GlobalShortcuts CreateShortcuts(DBusConnection connection)
        => new(connection, Bus, Path);

}
