// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ProcessData.Endpoints.Models;

public class NameIdentifierResponse
{
    public string Name { get; set; }
}

internal static class NameIdentifierResponseExtensions
{
    public static NameIdentifierResponse AsNameIdResponse(this string name)
        => new() { Name = name };
}