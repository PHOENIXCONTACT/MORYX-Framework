// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Operators.Attendances;

public interface IAttendanceManagementExtended : IAttendanceManagement
{
    event EventHandler<SignInStatusChangedArgs> SignInStatusChanged;

    void NotifyResource(IOperatorAssignable resource);
}
