// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers;
using Moryx.AbstractionLayer.Resources;
using Moryx.Communication;
using Moryx.Serialization;
using System.Runtime.Serialization;

namespace Moryx.Resources.Samples
{
    [ResourceRegistration]
    public class TcpDriverSample : Driver
    {
        public IBinaryConnectionFactory ConnectionFactory { get; set; }

        [DataMember, EntrySerialize]
        [PluginConfigs(typeof(IBinaryConnection))]
        public BinaryConnectionConfig TcpConfig { get; set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            var connection = ConnectionFactory.Create(TcpConfig, null);
        }
    }
}

