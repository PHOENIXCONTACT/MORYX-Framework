using Microsoft.Extensions.Logging;
using Moryx.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Moryx.Runtime.Kernel.Logging
{
    internal class ModuleLoggerFactory : IModuleLoggerFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public ModuleLoggerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public IModuleLogger Create(string name)
        {
            throw new NotImplementedException();
        }
    }
}
