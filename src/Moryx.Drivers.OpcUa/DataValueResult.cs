// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Opc.Ua;
using Moryx.Tools;

namespace Moryx.Drivers.OpcUa;

internal class DataValueResult : FunctionResult<DataValue>
{
    public DataValueResult(DataValue dataValue) : base(dataValue)
    {
    }

    public DataValueResult(FunctionResultError error) : base(error)
    {
    }

    internal static DataValueResult WithError(string message)
        => new(new FunctionResultError(message));

    internal static DataValueResult WithError(Exception exception)
        => new(new FunctionResultError(exception));
}
