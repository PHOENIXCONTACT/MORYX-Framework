// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Operators.Attendances;

namespace Moryx.Operators.Endpoints.Models;

internal sealed class SignInStatusChangedModel
{
    public required SignInStatus Status { get; set; }
    public required ExtendedOperatorModel Operator { get; set; }
    public required AttendableResourceModel Assignable { get; set; }
}
