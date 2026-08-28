// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Endpoints.Model;

public class OrderReferenceModel : ReferenceModel
{
    public required int ReferenceStateKey { get; set; }

    public required string ReferenceStateDisplayName { get; set; }

    public required string OrderNumber { get; set; }

    public string? OperationNumber { get; set; }

    public int StatusKey { get; set; }

    public string? StatusDisplayName { get; set; }

    public string? OperationSourceType { get; set; }
}