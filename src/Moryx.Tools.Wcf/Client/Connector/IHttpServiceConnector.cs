// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Modules;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// Interface for http service connectors
    /// </summary>
    [Obsolete("Use IWebServiceConnector from Moryx.Communication.Endpoints")]
    public interface IHttpServiceConnector : IPlugin
    {
        /// <summary>
        /// Informs that the availability of the client has changed
        /// </summary>
        event EventHandler AvailabilityChanged;

        /// <summary>
        /// Property the check the availability of the client
        /// </summary>
        bool IsAvailable { get; }
    }
}
