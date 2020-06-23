// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;

namespace Moryx.DependentTestModule
{
    /// <summary>
    /// Factory create named service manager instance
    /// </summary>
    [PluginFactory(typeof(INameBasedComponentSelector))]
    public interface ISimpleHelloWorldWcfSvcMgrFactory
    {
        /// <summary>
        /// Create named instance
        /// </summary>
        /// <param name="name">Name of implementation</param>
        /// <returns>Implementaion of <see cref="ISimpleHelloWorldWcfSvcMgr"/></returns>
        ISimpleHelloWorldWcfSvcMgr Create(string name);

        /// <summary>
        /// Destroy no longer needed instance
        /// </summary>
        void Destroy(ISimpleHelloWorldWcfSvcMgr instance);
    }
}
