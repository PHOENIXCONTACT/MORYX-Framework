// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ServiceModel;
using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel;
using Moryx.Tools.Wcf;

namespace Moryx.Runtime.Wcf
{
    internal class TypedHostFactory : DefaultServiceHostFactory, ITypedHostFactory
    {
        private readonly IKernel _kernel;
        public TypedHostFactory(IKernel kernel)
            : base(kernel)
        {
            _kernel = kernel;
        }

        public ServiceHost CreateServiceHost<T>()
        {
            // Get implementation type of service
            var handler = _kernel.GetHandler(typeof (T));
            var compModel = handler.ComponentModel;
            return base.CreateServiceHost(compModel.Implementation, new Uri[0]);
        }
    }
}