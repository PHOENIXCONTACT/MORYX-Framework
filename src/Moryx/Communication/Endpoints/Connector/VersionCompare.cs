// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Communication.Endpoints
{
    /// <summary>
    /// Helper class to compare major minor version of a version
    /// </summary>
    public static class VersionCompare
    {
        public static bool ClientMatch(Version server, Version client)
        {
            return server.Major == client.Major & server.Minor >= client.Minor;
        }
    }
}