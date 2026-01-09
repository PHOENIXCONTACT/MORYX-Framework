// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Endpoints.Databases.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
