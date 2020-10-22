// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ServiceModel;
using System.ServiceModel.Channels;
using Moryx.Tools.Wcf.Tests.Logging;

namespace Moryx.Tools.Wcf.Tests
{
    public class LogMaintenanceClientMock : LogMaintenanceClient
    {

        public LogMaintenanceClientMock(Binding binding, EndpointAddress remoteAddress):
                base(binding, remoteAddress) {
        }

        public new void Open()
        {

        }
    }
}
