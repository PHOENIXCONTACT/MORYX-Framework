// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("Moryx.Resources.IntegrationTests")]
[assembly: InternalsVisibleTo("Moryx.Resources.AssemblyInstruction.Tests")]
[assembly: InternalsVisibleTo("Moryx.Resources.Assemble.Tests")]
// We need this to host the legacy endpoint from the module
[assembly: InternalsVisibleTo("Moryx.ControlSystem.WorkerSupport")]