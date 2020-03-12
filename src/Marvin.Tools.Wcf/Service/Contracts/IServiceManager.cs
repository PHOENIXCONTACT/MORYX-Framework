// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// Basic interface for web serviuce managers.
    /// </summary>
    public interface IServiceManager : IDisposable
    {
        /// <summary>
        /// Register client on servicehost
        /// </summary>
        /// <param name="service"></param>
        void Register(ISessionService service);

        /// <summary>
        /// Unregister client from servicehost
        /// </summary>
        /// <param name="service"></param>
        void Unregister(ISessionService service);
    }
}
