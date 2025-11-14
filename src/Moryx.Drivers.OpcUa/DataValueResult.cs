// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG

using Moryx.Tools;
using Opc.Ua;

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
