// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.AbstractionLayer.Tests.TestData;
using NUnit.Framework;

namespace Moryx.AbstractionLayer.Tests;

[TestFixture]
public class ProcessExtensionsTests
{
    [TestCase(true, typeof(TestProductType))]
    [TestCase(false, null)]
    public void GetProductType_WhenRecipeIsResolved_ReturnsExpectedProductType(bool useProductRecipe, Type expectedType)
    {
        // Arrange
        var process = CreateProcess(useProductRecipe);

        // Act
        var productType = process.GetProductType();

        // Assert
        if (expectedType == null)
        {
            Assert.That(productType, Is.Null);
        }
        else
        {
            Assert.That(productType, Is.TypeOf(expectedType));
        }
    }

    [TestCase(true, true)]
    [TestCase(false, false)]
    public void GetProductType_WhenTargetIsResolved_ReturnsExpectedProductType(bool useExpectedType, bool shouldResolveProductType)
    {
        // Arrange
        ProductType productType = useExpectedType ? new TestProductType() : new OtherTestProductType();
        var process = CreateProductionProcess(productType, productType.CreateInstance());

        // Act
        var typedProductType = process.GetProductType<TestProductType>();

        // Assert
        if (shouldResolveProductType)
        {
            Assert.That(typedProductType, Is.SameAs(productType));
        }
        else
        {
            Assert.That(typedProductType, Is.Null);
        }
    }

    [TestCase(true, true)]
    [TestCase(false, false)]
    public void GetProductInstance_WhenProcessIsResolved_ReturnsExpectedProductInstance(bool useProductionProcess, bool shouldResolveProductInstance)
    {
        // Arrange
        var instance = CreateProductInstance();
        IProcess process = useProductionProcess ? CreateProductionProcess(instance.Type, instance)
            : CreateProcess(useProductRecipe: false);

        // Act
        var productInstance = process.GetProductInstance();

        // Assert
        if (shouldResolveProductInstance)
        {
            Assert.That(productInstance, Is.SameAs(instance));
        }
        else
        {
            Assert.That(productInstance, Is.Null);
        }
    }

    [TestCase(true, true)]
    [TestCase(false, false)]
    public void GetProductInstance_WhenInstanceIsResolved_ReturnsExpectedProductInstance(bool useExpectedInstance, bool shouldResolveProductInstance)
    {
        // Arrange
        var instance = useExpectedInstance ? CreateProductInstance()
            : new OtherTestProductType().CreateInstance();
        var process = CreateProductionProcess(instance.Type, instance);

        // Act
        var typedProductInstance = process.GetProductInstance<TestProductInstance>();

        // Assert
        if (shouldResolveProductInstance)
        {
            Assert.That(typedProductInstance, Is.SameAs(instance));
        }
        else
        {
            Assert.That(typedProductInstance, Is.Null);
        }
    }

    [Test]
    public void Modify_WhenProcessContainsExpectedInstance_UpdatesAndReturnsProductInstance()
    {
        // Arrange
        var instance = CreateProductInstance();
        var process = CreateProductionProcess(instance.Type, instance);

        // Act
        var modifiedInstance = process.Modify<TestProductInstance>(product => product.SerialNumber = "12345");

        // Assert
        Assert.That(modifiedInstance, Is.SameAs(instance));
        Assert.That(instance.SerialNumber, Is.EqualTo("12345"));
    }

    [TestCase(ProcessKind.Standard, typeof(InvalidOperationException))]
    [TestCase(ProcessKind.ProductionWithOtherInstance, typeof(InvalidCastException))]
    public void Modify_WhenProcessCannotProvideExpectedInstance_ThrowsExpectedException(ProcessKind processKind, Type exceptionType)
    {
        // Arrange
        var process = CreateProcess(processKind);

        // Act & Assert
        Assert.Throws(exceptionType, () => process.Modify<TestProductInstance>(_ => { }));
    }

    [TestCase(ProcessKind.ProductionWithExpectedInstance, true)]
    [TestCase(ProcessKind.Standard, false)]
    [TestCase(ProcessKind.ProductionWithOtherInstance, false)]
    public void TryModify_WhenProcessIsResolved_ReturnsExpectedResult(ProcessKind processKind, bool expectedResult)
    {
        // Arrange
        var process = CreateProcess(processKind);
        var expectedSerialNumber = "12345";

        // Act
        var result = process.TryModify<TestProductInstance>(product =>
        {
            product.SerialNumber = expectedSerialNumber;
        });

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
        Assert.That(process.GetProductInstance<TestProductInstance>()?.SerialNumber,
            Is.EqualTo(expectedResult ? expectedSerialNumber : null));
    }

    [TestCase(ActivityQuery.Next, 4)]
    [TestCase(ActivityQuery.Current, 5)]
    [TestCase(ActivityQuery.LastCompleted, 6)]
    public void ActivitySelector_WhenMatchingActivitiesExist_ReturnsExpectedActivity(ActivityQuery query, long expectedActivityId)
    {
        // Arrange
        var process = CreateProcessWithActivityStates();

        // Act
        var activity = SelectActivity(process, query);

        // Assert
        Assert.That(activity.Id, Is.EqualTo(expectedActivityId));
    }

    [TestCase(ActivityQuery.Next)]
    [TestCase(ActivityQuery.Current)]
    [TestCase(ActivityQuery.LastCompleted)]
    public void ActivitySelector_WhenNoMatchingActivityExists_ReturnsNull(ActivityQuery query)
    {
        // Arrange
        var process = new Process();

        // Act
        var activity = SelectActivity(process, query);

        // Assert
        Assert.That(activity, Is.Null);
    }

    [TestCase(ActivityGroup.Next, new long[] { 1, 4 })]
    [TestCase(ActivityGroup.Current, new long[] { 2, 5 })]
    public void ActivityGroupSelector_WhenMatchingActivitiesExist_ReturnsExpectedActivities(ActivityGroup group, long[] expectedActivityIds)
    {
        // Arrange
        var process = CreateProcessWithActivityStates();

        // Act
        var activityIds = SelectActivities(process, group).Select(activity => activity.Id).ToArray();

        // Assert
        Assert.That(activityIds, Is.EqualTo(expectedActivityIds));
    }

    [TestCase(nameof(TestActivity), 4)]
    [TestCase(nameof(DerivedTestActivity), 3)]
    [TestCase(nameof(OtherTestActivity), 2)]
    [TestCase("UnknownActivity", null)]
    public void LastActivityByTypeName_WhenTypeNameIsProvided_ReturnsExpectedActivity(string typeName, long? expectedActivityId)
    {
        // Arrange
        var process = CreateProcessWithActivityTypes();

        // Act
        var activity = process.LastActivity(typeName);

        // Assert
        if (expectedActivityId.HasValue)
        {
            Assert.That(activity.Id, Is.EqualTo(expectedActivityId.Value));
        }
        else
        {
            Assert.That(activity, Is.Null);
        }
    }

    [TestCase(false, 3)]
    [TestCase(true, 1)]
    public void LastActivityOfType_WhenExactFlagIsProvided_ReturnsExpectedActivity(bool exact, long expectedActivityId)
    {
        // Arrange
        var process = CreateProcessWithDerivedActivityLast();

        // Act
        var activity = process.LastActivity<TestActivity>(exact);

        // Assert
        Assert.That(activity.Id, Is.EqualTo(expectedActivityId));
    }

    [Test]
    public void LastActivityOfType_WhenExactFlagIsOmitted_IncludesDerivedActivities()
    {
        // Arrange
        var process = CreateProcessWithDerivedActivityLast();

        // Act
        var activity = process.LastActivity<TestActivity>();

        // Assert
        Assert.That(activity.Id, Is.EqualTo(3));
    }

    private static Process CreateProcess(bool useProductRecipe) => new()
    {
        Recipe = useProductRecipe ? new ProductRecipe { Product = new TestProductType() }
                : new TestRecipe()
    };

    private static Process CreateProcess(ProcessKind processKind) => processKind switch
    {
        ProcessKind.ProductionWithExpectedInstance => CreateProductionProcess(new TestProductType()),
        ProcessKind.ProductionWithOtherInstance => CreateProductionProcess(new OtherTestProductType()),
        _ => new Process()
    };

    private static ProductionProcess CreateProductionProcess(ProductType productType) =>
        CreateProductionProcess(productType, productType.CreateInstance());

    private static ProductionProcess CreateProductionProcess(ProductType productType, ProductInstance productInstance)
    {
        return new()
        {
            Recipe = new ProductRecipe { Product = productType },
            ProductInstance = productInstance
        };
    }

    private static TestProductInstance CreateProductInstance() =>
        (TestProductInstance)new TestProductType().CreateInstance();

    private static Process CreateProcessWithActivityStates()
    {
        var process = new Process();
        process.AddActivity(CreateActivity(1));
        process.AddActivity(CreateActivity(2, started: true));
        process.AddActivity(CreateActivity(3, started: true, completed: true));
        process.AddActivity(CreateActivity(4));
        process.AddActivity(CreateActivity(5, started: true));
        process.AddActivity(CreateActivity(6, started: true, completed: true));
        return process;
    }

    private static Process CreateProcessWithActivityTypes()
    {
        var process = new Process();
        process.AddActivity(CreateActivity<TestActivity>(1));
        process.AddActivity(CreateActivity<OtherTestActivity>(2));
        process.AddActivity(CreateActivity<DerivedTestActivity>(3));
        process.AddActivity(CreateActivity<TestActivity>(4));
        return process;
    }

    private static Process CreateProcessWithDerivedActivityLast()
    {
        var process = new Process();
        process.AddActivity(CreateActivity<TestActivity>(1));
        process.AddActivity(CreateActivity<OtherTestActivity>(2));
        process.AddActivity(CreateActivity<DerivedTestActivity>(3));
        return process;
    }

    private static TestActivity CreateActivity(long id, bool started = false, bool completed = false)
    {
        var activity = CreateActivity<TestActivity>(id);

        if (started)
        {
            activity.Tracing.Started = DateTime.Now;
        }

        if (completed)
        {
            activity.Result = ActivityResult.Create(true, 0);
        }

        return activity;
    }

    private static TActivity CreateActivity<TActivity>(long id) where TActivity : Activity, new() => new() { Id = id };

    private static Activity SelectActivity(IProcess process, ActivityQuery query) => query switch
    {
        ActivityQuery.Next => process.NextActivity(),
        ActivityQuery.Current => process.CurrentActivity(),
        _ => process.LastActivity()
    };

    private static Activity[] SelectActivities(IProcess process, ActivityGroup group) => group switch
    {
        ActivityGroup.Next => [.. process.NextActivities()],
        _ => [.. process.CurrentActivities()]
    };

    public enum ProcessKind
    {
        Standard,
        ProductionWithExpectedInstance,
        ProductionWithOtherInstance
    }

    public enum ActivityQuery
    {
        Next,
        Current,
        LastCompleted
    }

    public enum ActivityGroup
    {
        Next,
        Current
    }
}
