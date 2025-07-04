// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers;
using Moryx.AbstractionLayer.Resources;
using Moryx.Communication;
using Moryx.Container;
using Moryx.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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

