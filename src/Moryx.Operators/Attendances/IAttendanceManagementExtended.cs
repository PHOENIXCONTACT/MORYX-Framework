// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Operators.Attendances;

public interface IAttendanceManagementExtended : IAttendanceManagement
{

    /// <summary>
    /// Event to inform that an operator was signed in or signed out of a specific resource
    /// </summary>
    event EventHandler<SignInStatusChangedArgs>? SignInStatusChanged;

    /// <summary>
    /// Gets a list of assignable resources
    /// </summary>
    IEnumerable<IOperatorAssignable> Assignables { get; }

    /// <summary>
    /// Get's the assignable with the given id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    IOperatorAssignable GetAssignable(long id);

    /// <summary>
    /// Retrieves a list of signed in operators and their skills
    /// </summary>
    /// <param name="resource"></param>
    /// <returns></returns>
    IReadOnlyList<AttendanceChangedArgs> GetAttendingOperators(IOperatorAssignable resource);
}
