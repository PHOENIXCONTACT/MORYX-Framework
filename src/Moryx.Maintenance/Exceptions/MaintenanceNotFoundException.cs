// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Moryx.Maintenance.Properties;

namespace Moryx.Maintenance.Exceptions;

/// <summary>
/// Thrown when a maintenance order was not found
/// </summary>
public class MaintenanceNotFoundException(long id) : NotFoundException(string.Format(Thread.CurrentThread.CurrentCulture, Strings.MaintenanceNotFoundException_Message, id))
{
}
