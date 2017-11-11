using System;

namespace Marvin.Tools.Wcf
{
    internal class ServiceConfiguration
    {
        public BindingType BindingType { get; set; }

        public string ServiceUrl { get; set; }

        public string ServerVersion { get; set; }

        public bool RequiresAuthentification { get; set; }

        public string MinClientVersion { get; set; }

        internal ServiceConfiguration()
        {
            
        }

        internal ServiceConfiguration(ServiceConfig wcfDto)
        {
            ServiceUrl = wcfDto.ServiceUrl;
            ServerVersion = wcfDto.ServerVersion;
            MinClientVersion = wcfDto.MinClientVersion;
            RequiresAuthentification = wcfDto.RequiresAuthentification;
            
            switch (wcfDto.Binding)
            {
                case ServiceBindingType.BasicHttp:
                    BindingType = BindingType.BasicHttp;
                    break;
                case ServiceBindingType.NetTcp:
                    BindingType = BindingType.NetTcp;
                    break;
                default:
                    throw new ArgumentException(string.Format("Server binding type '{0}' not supported.", wcfDto.Binding));
            }  
        }
    }
}