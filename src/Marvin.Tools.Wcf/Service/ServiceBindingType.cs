// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// Type of binding
    /// </summary>
    public enum ServiceBindingType
    {
        /// <summary>
        /// WebHttp binding used for standard http methods and web clients
        /// </summary>
        WebHttp,

        /// <summary>
        /// Soap http binding
        /// </summary>
        BasicHttp,

        /// <summary>
        /// Net tcp binding for fast, session bound duplex connections
        /// </summary>
        NetTcp
    }
}
