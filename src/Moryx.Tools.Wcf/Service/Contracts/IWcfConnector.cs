// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// The public API of the WCF connector plugin.
    /// </summary>
    public interface IWcfConnector<TConfig> : IConfiguredPlugin<TConfig>
        where TConfig : IWcfServiceConfig
    {
         
    }
}
