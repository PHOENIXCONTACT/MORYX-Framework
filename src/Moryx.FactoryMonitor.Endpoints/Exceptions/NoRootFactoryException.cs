// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.FactoryMonitor.Endpoints.Exceptions;

public class NoRootFactoryException : Exception
{
    public NoRootFactoryException(string message) : base(message)
    {
    }
}