// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
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

        protected override async Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            await base.OnInitializeAsync(cancellationToken);

            var connection = ConnectionFactory.Create(TcpConfig, null);
        }
    }
}

