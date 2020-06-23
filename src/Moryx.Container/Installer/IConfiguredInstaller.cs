// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Container
{
    internal interface IConfiguredInstaller
    {
        /// <summary>
        /// Set config on installer
        /// </summary>
        void SetRegistrator(ComponentRegistrator config);
    }
}
