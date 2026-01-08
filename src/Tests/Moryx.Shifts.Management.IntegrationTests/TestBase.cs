// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moq;
using Moryx.AbstractionLayer.Resources;
using Moryx.Operators;
using Moryx.Operators.Attendances;
using Moryx.TestTools.IntegrationTest;
using Moryx.Tools;
using NUnit.Framework;

namespace Moryx.Shifts.Management.IntegrationTests
{
    [TestFixture]
    internal abstract class TestBase
    {
        protected IShiftManagement _facade;
        protected static Mock<IResourceManagement> _resourceManagementMock;
        protected static Mock<IOperatorManagement> _operatorManagementMock;
        protected MoryxTestEnvironment _env;

        public virtual Task SetUp()
        {
            ReflectionTool.TestMode = true;
            var config = new ModuleConfig();
            _resourceManagementMock = MoryxTestEnvironment.CreateModuleMock<IResourceManagement>();
            _operatorManagementMock = MoryxTestEnvironment.CreateModuleMock<IOperatorManagement>();
            _env = new MoryxTestEnvironment(typeof(ModuleController),
                [_resourceManagementMock, _operatorManagementMock], config);
            _facade = _env.GetTestModule<IShiftManagement>();

            return Task.CompletedTask;
        }

        public virtual Task TearDown()
        {
            return _env.StopTestModuleAsync();
        }

        #region Test tools 

        protected readonly ShiftTypeCreationContext TypeContext = new("testType") { StartTime = new TimeOnly(6, 0), EndTime = new TimeOnly(14, 0), Periode = 5 };
        protected readonly Mock<IResource> ResourceMock = CreateResourceMock();
        protected readonly AssignableOperator Operator = CreateOperator();

        protected static AssignableOperator CreateOperator() => new("Test Identifier") { FirstName = "MORYX", LastName = "Industry", Pseudonym = "xyz" };

        protected static Mock<IResource> CreateResourceMock() => new();

        protected static ShiftAssignementCreationContext GetAssignementContext(Shift shift, IResource resource, Operator @operator) =>
            new(shift, resource, @operator) { Priority = 42, AssignedDays = AssignedDays.Second & AssignedDays.Third & AssignedDays.Fourth & AssignedDays.Sixth };

        protected static ShiftCreationContext GetShiftContext(ShiftType shiftType) =>
            new(shiftType) { Date = new DateOnly(2024, 1, 1) };

        protected static bool IsObjectMatchingContext(ShiftTypeCreationContext context, ShiftType type) =>
            type.Name == context.Name && type.StartTime == context.StartTime &&
            type.Endtime == context.EndTime && type.Periode == context.Periode;

        protected static bool IsObjectMatchingContext(ShiftCreationContext context, Shift shift) =>
            context.Type.Id == shift.Type.Id && context.Date == shift.Date;

        protected static bool IsObjectMatchingContext(ShiftAssignementCreationContext context, ShiftAssignement assignement) =>
            ObjectsAreEqual(assignement.Shift, context.Shift) && context.Operator.Identifier == assignement.Operator.Identifier &&
            context.Resource.Id == assignement.Resource.Id && context.Note == assignement.Note && context.Priority == assignement.Priority &&
            context.AssignedDays == assignement.AssignedDays;

        protected static bool ObjectsAreEqual(ShiftType updatedShiftType, ShiftType result) =>
            result is not null && updatedShiftType is not null && updatedShiftType.Id == result.Id &&
            updatedShiftType.Name == result.Name && updatedShiftType.StartTime == result.StartTime &&
            updatedShiftType.Endtime == result.Endtime && updatedShiftType.Periode == result.Periode;

        protected static bool ObjectsAreEqual(Shift updatedShift, Shift result) =>
            result is not null && updatedShift is not null && updatedShift.Id == result.Id &&
            updatedShift.Date == result.Date && ObjectsAreEqual(updatedShift.Type, result.Type);

        protected static bool ObjectsAreEqual(ShiftAssignement updatedAssignement, ShiftAssignement result) =>
            updatedAssignement is not null && result is not null && updatedAssignement.Id == result.Id &&
            updatedAssignement.Shift.Id == result.Shift.Id && updatedAssignement.Operator == result.Operator &&
            updatedAssignement.Resource == result.Resource && updatedAssignement.Note == result.Note &&
            updatedAssignement.Priority == result.Priority && updatedAssignement.AssignedDays == result.AssignedDays;

        #endregion
    }
}

