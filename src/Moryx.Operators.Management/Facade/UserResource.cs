// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Resources;
using Moryx.Operators.Attendances;

namespace Moryx.Operators.Management;

internal class UserResource : Resource, IOperatorAssignable
{
    public static UserResource Instance => new();

    private UserResource()
    {
        Id = -9999;
        Name = "Used Resource";
        Description = "Internal resource to sign in users using the deprecated IUserManagement facade";
    }

    public ICapabilities RequiredSkills => NullCapabilities.Instance;

    public void AttendanceChanged(IReadOnlyList<AttendanceChangedArgs> attandances) { }
}

