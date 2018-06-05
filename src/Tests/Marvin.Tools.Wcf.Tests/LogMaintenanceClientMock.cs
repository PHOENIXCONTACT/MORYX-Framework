using Marvin.Tools.Wcf.Tests.Logging;

namespace Marvin.Tools.Wcf.Tests
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