// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using NUnit.Framework;

namespace Moryx.Shifts.Management.IntegrationTests
{
    [TestFixture]
    internal class FacadeTests : TestBase
    {
        [SetUp]
        public override Task SetUp()
        {
            base.SetUp();
            return _env.StartTestModuleAsync();
        }

        [TearDown]
        public override Task TearDown()
        {
            return base.TearDown();
        }

        #region Shift Types

        [Test]
        public void CreateShiftType_WithContext_ReturnsShiftType()
        {
            // Arrange
            // Act
            var type = _facade.CreateShiftType(TypeContext);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(type, Is.Not.Null);
                Assert.That(type.Id, Is.GreaterThan(0));
                Assert.That(IsObjectMatchingContext(TypeContext, type));
            });
        }

        [Test]
        public void CreateShiftType_WithContext_RaisesEvent()
        {
            // Arrange
            ShiftTypesChangedEventArgs eventArgs = null;
            _facade.TypesChanged += (object sender, ShiftTypesChangedEventArgs e) => eventArgs = e;

            // Act
            _facade.CreateShiftType(TypeContext);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(eventArgs, Is.Not.Null);
                Assert.That(eventArgs.Change, Is.EqualTo(ShiftTypeChange.Creation));
                Assert.That(eventArgs.Type.Id, Is.GreaterThan(0));
                Assert.That(IsObjectMatchingContext(TypeContext, eventArgs.Type));
            });
        }

        [Test]
        public void CreateShiftType_WithNull_ThrowsArumentNullException()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => _facade.CreateShiftType(null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void UpdateShiftType_WithValidShiftType_UpdatesShiftType()
        {
            // Arrange
            var shiftType = _facade.CreateShiftType(TypeContext);
            var updatedShiftType = Update(shiftType);

            // Act
            _facade.UpdateShiftType(shiftType);

            // Assert
            var result = _facade.ShiftTypes.SingleOrDefault(t => t.Id == shiftType.Id);
            Assert.That(ObjectsAreEqual(updatedShiftType, result));
        }

        private static ShiftType Update(ShiftType shiftType)
        {
            shiftType.Name = "New name";
            shiftType.StartTime = new TimeOnly(14, 0);
            shiftType.Endtime = new TimeOnly(22, 0);
            shiftType.Periode = 1;
            return shiftType;
        }

        [Test]
        public void UpdateShiftType_WithValidShiftType_RaisesEvent()
        {
            // Arrange
            var shiftType = _facade.CreateShiftType(TypeContext);
            var updatedShiftType = Update(shiftType);
            ShiftTypesChangedEventArgs eventArgs = null;
            _facade.TypesChanged += (object sender, ShiftTypesChangedEventArgs e) => eventArgs = e;

            // Act
            _facade.UpdateShiftType(shiftType);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(eventArgs, Is.Not.Null);
                Assert.That(eventArgs.Change, Is.EqualTo(ShiftTypeChange.Update));
                Assert.That(ObjectsAreEqual(updatedShiftType, eventArgs.Type));
            });
        }

        [TestCaseSource(nameof(InvalidArgumentsForUpdateShiftType))]
        public void UpdateShiftType_WithInvalidArgument_ThrowsException(ShiftType type, Type exception)
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => _facade.UpdateShiftType(type), Throws.InstanceOf(exception));
        }

        private static readonly object[] InvalidArgumentsForUpdateShiftType = [
            new object[] { null, typeof(ArgumentNullException) }, // , $"Throws {nameof(ArgumentNullException)}"
            new object[] { new ShiftType("Incomplete type"), typeof(ArgumentException) }, // , $"Throws {nameof(ArgumentException)} on invalid inputs"
        ];

        [Test]
        public void DeleteShiftType_WithValidId_RemovesShiftType()
        {
            // Arrange
            var shiftType = _facade.CreateShiftType(TypeContext);

            // Act
            _facade.DeleteShiftType(shiftType.Id);

            // Assert
            Assert.That(_facade.ShiftTypes.Any(t => t.Id == shiftType.Id), Is.False);
        }

        [Test]
        public void DeleteShiftType_WithValidId_RaisesEvent()
        {
            // Arrange
            var shiftType = _facade.CreateShiftType(TypeContext);
            ShiftTypesChangedEventArgs eventArgs = null;
            _facade.TypesChanged += (object sender, ShiftTypesChangedEventArgs e) => eventArgs = e;

            // Act
            _facade.DeleteShiftType(shiftType.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(eventArgs, Is.Not.Null);
                Assert.That(eventArgs.Change, Is.EqualTo(ShiftTypeChange.Deletion));
                Assert.That(ObjectsAreEqual(shiftType, eventArgs.Type));
            });
        }

        [Test]
        public void DeleteShiftType_WithInvalidArgument_ThrowsException()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => _facade.DeleteShiftType(0), Throws.TypeOf<ArgumentException>());
        }

        #endregion

        #region Shifts

        [Test]
        public void CreateShift_WithContext_ReturnsShift()
        {
            // Arrange
            var shiftType = _facade.CreateShiftType(TypeContext);
            var context = GetShiftContext(shiftType);

            // Act
            var shift = _facade.CreateShift(context);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(shift, Is.Not.Null);
                Assert.That(shift.Id, Is.GreaterThan(0));
                Assert.That(IsObjectMatchingContext(context, shift));
            });
        }

        [Test]
        public void CreateShift_WithContext_RaisesEvent()
        {
            // Arrange
            var shiftType = _facade.CreateShiftType(TypeContext);
            var context = GetShiftContext(shiftType);
            ShiftsChangedEventArgs eventArgs = null;
            _facade.ShiftsChanged += (object sender, ShiftsChangedEventArgs e) => eventArgs = e;

            // Act
            _facade.CreateShift(context);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(eventArgs, Is.Not.Null);
                Assert.That(eventArgs.Change, Is.EqualTo(ShiftChange.Creation));
                Assert.That(eventArgs.Shift.Id, Is.GreaterThan(0));
                Assert.That(IsObjectMatchingContext(context, eventArgs.Shift));
            });
        }

        [TestCaseSource(nameof(InvalidArgumentsForCreateShift))]
        public void CreateShift_WithInvalidArguments_ThrowsException(ShiftCreationContext context, Type exception)
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => _facade.CreateShift(context), Throws.InstanceOf(exception));
        }

        private static readonly object[] InvalidArgumentsForCreateShift = [
            new object[] { null, typeof(ArgumentNullException) }, // ArgumentNullException on null parameter
            new object[] { new ShiftCreationContext(new ShiftType("Unknown Type")), typeof(ArgumentException) }, // ArgumentException on invalid inputs
            new object[] { new ShiftCreationContext(null), typeof(ArgumentException) }, // ArgumentException on invalid inputs
        ];

        [Test]
        public void UpdateShift_WithValidShift_UpdatesShift()
        {
            // Arrange
            var shiftType = _facade.CreateShiftType(TypeContext);
            var newShiftType = _facade.CreateShiftType(TypeContext);
            var context = GetShiftContext(shiftType);
            var shift = _facade.CreateShift(context);
            var updatedShift = Update(shift, newShiftType);

            // Act
            _facade.UpdateShiftType(shiftType);

            // Assert
            var result = _facade.Shifts.SingleOrDefault(t => t.Id == shift.Id);
            Assert.That(ObjectsAreEqual(updatedShift, result));
        }

        private static Shift Update(Shift shift, ShiftType type)
        {
            shift.Type = type;
            shift.Date = new DateOnly(2000, 1, 1);
            return shift;
        }

        [Test]
        public void UpdateShift_WithValidShift_RaisesEvent()
        {
            // Arrange
            var shiftType = _facade.CreateShiftType(TypeContext);
            var newShiftType = _facade.CreateShiftType(TypeContext);
            var context = GetShiftContext(shiftType);
            var shift = _facade.CreateShift(context);
            var updatedShift = Update(shift, newShiftType);
            ShiftsChangedEventArgs eventArgs = null;
            _facade.ShiftsChanged += (object sender, ShiftsChangedEventArgs e) => eventArgs = e;

            // Act
            _facade.UpdateShift(updatedShift);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(eventArgs, Is.Not.Null);
                Assert.That(eventArgs.Change, Is.EqualTo(ShiftChange.Update));
                Assert.That(ObjectsAreEqual(updatedShift, eventArgs.Shift));
            });
        }

        [TestCaseSource(nameof(InvalidArgumentsForUpdateShift))]
        public void UpdateShift_WithInvalidArgument_ThrowsException(Shift type, Type exception, string message)
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => _facade.UpdateShift(type), Throws.InstanceOf(exception), message);
        }

        private static readonly object[] InvalidArgumentsForUpdateShift = [
            new object[] { null, typeof(ArgumentNullException), "ArgumentNullException on null parameter" },
            new object[] { new Shift(new ShiftType("Incomplete type")) { Id = 1 }, typeof(ArgumentException), "ArgumentException on unknown inputs" },
            new object[] { new Shift(null) { Id = 1 }, typeof(ArgumentException), "ArgumentException on invalid inputs" }
        ];

        [Test]
        public void DeleteShift_WithValidId_RemovesShift()
        {
            // Arrange
            var shiftType = _facade.CreateShiftType(TypeContext);
            var context = GetShiftContext(shiftType);
            var shift = _facade.CreateShift(context);

            // Act
            _facade.DeleteShift(shift.Id);

            // Assert
            Assert.That(_facade.Shifts.Any(s => s.Id == shift.Id), Is.False);
        }

        [Test]
        public void DeleteShift_WithValidId_RaisesEvent()
        {
            // Arrange
            var shiftType = _facade.CreateShiftType(TypeContext);
            var context = GetShiftContext(shiftType);
            var shift = _facade.CreateShift(context);
            ShiftsChangedEventArgs eventArgs = null;
            _facade.ShiftsChanged += (object sender, ShiftsChangedEventArgs e) => eventArgs = e;

            // Act
            _facade.DeleteShiftType(shiftType.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(eventArgs, Is.Not.Null);
                Assert.That(eventArgs.Change, Is.EqualTo(ShiftChange.Deletion));
                Assert.That(ObjectsAreEqual(shift, eventArgs.Shift));
            });
        }

        [Test]
        public void DeleteShift_WithInvalidArgument_ThrowsException()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => _facade.DeleteShift(0), Throws.TypeOf<ArgumentException>());
        }

        #endregion

        #region Shift Assignements

        [Test]
        public void CreateShiftAssignement_WithContext_ReturnsShiftAssignement()
        {
            // Arrange
            var shiftType = _facade.CreateShiftType(TypeContext);
            var shift = _facade.CreateShift(GetShiftContext(shiftType));
            var context = GetAssignementContext(shift, ResourceMock.Object, Operator);

            // Act
            var assignement = _facade.CreateShiftAssignement(context);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(assignement, Is.Not.Null);
                Assert.That(assignement.Id, Is.GreaterThan(0));
                Assert.That(IsObjectMatchingContext(context, assignement));
            });
        }

        [Test]
        public void CreateShiftAssignement_WithContext_RaisesEvent()
        {
            // Arrange
            var shiftType = _facade.CreateShiftType(TypeContext);
            var shift = _facade.CreateShift(GetShiftContext(shiftType));
            var context = GetAssignementContext(shift, ResourceMock.Object, Operator);
            ShiftAssignementsChangedEventArgs eventArgs = null;
            _facade.AssignementsChanged += (object sender, ShiftAssignementsChangedEventArgs e) => eventArgs = e;

            // Act
            _facade.CreateShiftAssignement(context);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(eventArgs, Is.Not.Null);
                Assert.That(eventArgs.Change, Is.EqualTo(ShiftAssignementChange.Creation));
                Assert.That(eventArgs.Assignement.Id, Is.GreaterThan(0));
                Assert.That(IsObjectMatchingContext(context, eventArgs.Assignement));
            });
        }

        [TestCaseSource(nameof(InvalidArgumentsForCreateShiftAssignement))]
        public void CreateShiftAssignement_WithInvalidArguments_ThrowsException(ShiftAssignementCreationContext context, Type exception)
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => _facade.CreateShiftAssignement(context), Throws.InstanceOf(exception));
        }

        private static readonly object[] InvalidArgumentsForCreateShiftAssignement = [
            new object[] { null, typeof(ArgumentNullException) }, // Argument is null
            new object[] { new ShiftAssignementCreationContext(new Shift(new ShiftType("Unknown Type")), CreateResourceMock().Object, CreateOperator()), typeof(ArgumentException) }, // Context has invalid Shift
        ];

        [Test]
        public void DeleteShiftAssignement_WithValidId_RemovesShiftAssignement()
        {
            // Arrange
            var shiftType = _facade.CreateShiftType(TypeContext);
            var shift = _facade.CreateShift(GetShiftContext(shiftType));
            var context = GetAssignementContext(shift, ResourceMock.Object, Operator);
            var assignement = _facade.CreateShiftAssignement(context);

            // Act
            _facade.DeleteShiftAssignement(assignement.Id);

            // Assert
            Assert.That(_facade.ShiftAssignements.Any(a => a.Id == shift.Id), Is.False);
        }

        [Test]
        public void DeleteShiftAssignement_WithValidId_RaisesEvent()
        {
            // Arrange
            var shiftType = _facade.CreateShiftType(TypeContext);
            var shift = _facade.CreateShift(GetShiftContext(shiftType));
            var context = GetAssignementContext(shift, ResourceMock.Object, Operator);
            var assignement = _facade.CreateShiftAssignement(context);
            ShiftAssignementsChangedEventArgs eventArgs = null;
            _facade.AssignementsChanged += (object sender, ShiftAssignementsChangedEventArgs e) => eventArgs = e;

            // Act
            _facade.DeleteShiftAssignement(assignement.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(eventArgs, Is.Not.Null);
                Assert.That(eventArgs.Change, Is.EqualTo(ShiftAssignementChange.Deletion));
                Assert.That(ObjectsAreEqual(assignement, eventArgs.Assignement));
            });
        }

        [Test]
        public void DeleteShiftAssignement_WithInvalidArgument_ThrowsException()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => _facade.DeleteShift(0), Throws.TypeOf<ArgumentException>());
        }

        #endregion
    }
}

