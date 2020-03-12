// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// Host factory to create dependency injection enabled web services
    /// </summary>
    public interface IConfiguredHostFactory
    {
        /// <summary>
        /// Create host instance for a wcf service
        /// </summary>
        IConfiguredServiceHost CreateHost<T>(HostConfig config);
    }
}
