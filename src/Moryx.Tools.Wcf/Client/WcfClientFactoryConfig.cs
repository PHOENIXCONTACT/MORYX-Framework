// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// Public API of the WCF client factory configuration.
    /// </summary>
    public class WcfClientFactoryConfig : IWcfClientFactoryConfig
    {
        /// 
        public string Host { get; set; }

        /// 
        public int Port { get; set; }

        /// 
        public string ClientId { get; set; }
    }
}
