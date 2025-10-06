// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Operators.Extensions;

public static class IAttandanceManagementExtensions
{
    /// <summary>
    /// Returns the operator with the given identifier.
    /// </summary>
    /// <param name="identifier">Identifier of the operator</param>
    public static AssignableOperator? GetOperator(this IAttendanceManagement source, string identifier)
        => source.Operators.SingleOrDefault(o => o.Identifier == identifier);
}
