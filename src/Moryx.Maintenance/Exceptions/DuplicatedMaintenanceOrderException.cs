// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Moryx.Maintenance.Localizations;

namespace Moryx.Maintenance.Exceptions;

/// <summary>
/// Exception thrown for a duplicated maintenance Order for the same resource
/// </summary>
public class DuplicatedMaintenanceOrderException(string resource) : NotFoundException(string.Format(Thread.CurrentThread.CurrentCulture, Strings.DUPLICATED_RESOURCE_EXCEPTION, resource))
{
}
