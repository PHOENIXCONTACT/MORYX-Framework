// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Tools.Wcf.Tests.Logging;

namespace Moryx.Tools.Wcf.Tests
{
    public class LogMaintenanceClientMock : LogMaintenanceClient
    {
        public LogMaintenanceClientMock() {
        }

        public LogMaintenanceClientMock(string endpointConfigurationName) :
                base(endpointConfigurationName) {
        }

        public LogMaintenanceClientMock(string endpointConfigurationName, string remoteAddress) :
                base(endpointConfigurationName, remoteAddress) {
        }

        public LogMaintenanceClientMock(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
                base(endpointConfigurationName, remoteAddress) {
        }

        public LogMaintenanceClientMock(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
                base(binding, remoteAddress) {
        }

        public new void Open()
        {

        }
    }
}
