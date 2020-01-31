// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Tools.Wcf
{
    internal class ServiceConfiguration
    {
        public BindingType BindingType { get; set; }

        public string ServiceUrl { get; set; }

        public string ServerVersion { get; set; }

        public bool RequiresAuthentication { get; set; }

        public string MinClientVersion { get; set; }

        internal ServiceConfiguration()
        {

        }

        internal ServiceConfiguration(ServiceConfig wcfDto)
        {
            ServiceUrl = wcfDto.ServiceUrl;
            ServerVersion = wcfDto.ServerVersion;
            MinClientVersion = wcfDto.MinClientVersion;
            RequiresAuthentication = wcfDto.RequiresAuthentication;

            switch (wcfDto.Binding)
            {
                case ServiceBindingType.BasicHttp:
                    BindingType = BindingType.BasicHttp;
                    break;
                case ServiceBindingType.NetTcp:
                    BindingType = BindingType.NetTcp;
                    break;
                default:
                    throw new ArgumentException($"Server binding type '{wcfDto.Binding}' not supported.");
            }
        }
    }
}
