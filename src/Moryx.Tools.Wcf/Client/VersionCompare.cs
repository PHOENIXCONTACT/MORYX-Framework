using System;

namespace Moryx.Tools.Wcf
{
    internal static class VersionCompare
    {
        public static bool ClientMatch(Version server, Version client)
        {
            return server.Major == client.Major & server.Minor >= client.Minor;
        }
    }
}