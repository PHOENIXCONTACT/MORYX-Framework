// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Model.Configuration;

/// <summary>
/// Empty config for the <see cref="NullModelConfigurator"/>
/// </summary>
public class NullDatabaseConfig : DatabaseConfig
{
    /// <inheritdoc />
    public override string ConfiguratorType => typeof(NullModelConfigurator).AssemblyQualifiedName;
}
