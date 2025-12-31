// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.ControlSystem.ProcessEngine.Jobs;

/// <summary>
/// Controller component for managing preallocated jobs until their allocations are frozen
/// </summary>
internal interface IJobPreallocationController : IPlugin
{
}
