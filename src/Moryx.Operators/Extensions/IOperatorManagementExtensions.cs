// Copyright (c) 2024, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Operators.Extensions;

public static class IOperatorManagementExtensions
{
    /// <summary>
    /// Returns the operator with the given identifier.
    /// </summary>
    /// <param name="identifier">Identifier of the operator</param>
    public static Operator? GetOperator(this IOperatorManagement source, string identifier)
    {
        return source.Operators.SingleOrDefault(o => o.Identifier == identifier);
    }
}
