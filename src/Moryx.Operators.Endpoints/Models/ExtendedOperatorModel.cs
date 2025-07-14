﻿// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Operators.Endpoints;

public class ExtendedOperatorModel : OperatorModel
{
    public IEnumerable<ResourceModel>? AssignedResources { get; set; }
}

