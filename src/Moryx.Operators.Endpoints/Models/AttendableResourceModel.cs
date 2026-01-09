// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Operators.Endpoints.Models;

public class AttendableResourceModel
{
    public long? Id { get; set; }

    public string? Name { get; set; }

    public List<string>? RequiredSkills { get; set; }
}
