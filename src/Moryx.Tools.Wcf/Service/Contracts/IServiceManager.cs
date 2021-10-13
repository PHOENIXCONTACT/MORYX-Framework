// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// Basic interface for web service managers.
    /// </summary>
    public interface IServiceManager : IDisposable
    {
        /// <summary>
        /// Register client on service host
        /// </summary>
        /// <param name="service"></param>
        void Register(ISessionService service);

        /// <summary>
        /// Unregister client from service host
        /// </summary>
        /// <param name="service"></param>
        void Unregister(ISessionService service);
    }
}
