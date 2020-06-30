// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Marvin.Configuration;

namespace Marvin.Runtime.Kernel
{
    internal interface IConfigLiveUpdate
    {
        /// <summary>
        /// Update the active config object from the copy and invoke
        /// </summary>
        /// <param name="sharedType">Type of both objects</param>
        /// <param name="activeConfig"></param>
        /// <param name="modifiedCopy"></param>
        void UpdateLive(Type sharedType, IConfig activeConfig, IConfig modifiedCopy);
    }
}
