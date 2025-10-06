// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.Jobs.Endpoints
{
    public static class JobPermissions
    {
        private const string _prefix = "Moryx.Jobs.";
        public const string CanView = _prefix + "CanView";
        public const string CanComplete = _prefix + "CanComplete";
        public const string CanAbort = _prefix + "CanAbort";
    }

}

