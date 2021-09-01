// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Microsoft.EntityFrameworkCore;
using Moryx.Configuration;
using Moryx.Logging;

namespace Moryx.Model.Configuration
{
    /// <summary>
    /// Null implementation of the <see cref="ModelConfiguratorBase{TConfig}"/>
    /// </summary>
    public sealed class NullModelConfigurator : IModelConfigurator
    {
        /// <inheritdoc />
        public IDatabaseConfig Config => null;

        /// <inheritdoc />
        public void Initialize(Type contextType, IConfigManager configManager, IModuleLogger logger)
        {
        }

        /// <inheritdoc />
        public DbContext CreateContext()
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public DbContext CreateContext(IDatabaseConfig config)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public void UpdateConfig()
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public TestConnectionResult TestConnection(IDatabaseConfig config)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public bool CreateDatabase(IDatabaseConfig config)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public void DeleteDatabase(IDatabaseConfig config)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public void DumpDatabase(IDatabaseConfig config, string targetPath)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }

        /// <inheritdoc />
        public void RestoreDatabase(IDatabaseConfig config, string filePath)
        {
            throw new NotSupportedException("Not supported by " + nameof(NullModelConfigurator));
        }
    }
}