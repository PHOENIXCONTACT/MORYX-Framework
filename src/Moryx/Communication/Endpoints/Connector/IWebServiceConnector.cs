// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Modules;

namespace Moryx.Communication.Endpoints
{
    /// <summary>
    /// Interface for http service connectors
    /// </summary>
    public interface IWebServiceConnector : IPlugin
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