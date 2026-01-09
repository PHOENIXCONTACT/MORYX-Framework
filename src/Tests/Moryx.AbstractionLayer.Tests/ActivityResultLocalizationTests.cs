// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Tests.TestData;
using NUnit.Framework;
using System.Linq;

namespace Moryx.AbstractionLayer.Tests;

[TestFixture]
public class ActivityResultLocalizationTests
{

    [Test]
    public void ActivityResultDisplayName()
    {
        //Arrange
        TestTask testTask = new TestTask();
        string expectedFailedDisplayName = "This is a failed result";
        string expectedSuccesDisplayName = TestResults.Success.ToString();

        //Assign
        var outputDescriptions = testTask.OutputDescriptions;
        var failedResultDescription = outputDescriptions
            .FirstOrDefault(x => x.MappingValue == (long)TestResults.Failed);
        var successResultDescription = outputDescriptions
            .FirstOrDefault(x => x.MappingValue == (long)TestResults.Success);

        //assert
        Assert.That(expectedFailedDisplayName, Is.EqualTo(failedResultDescription.Name));
        Assert.That(expectedSuccesDisplayName, Is.EqualTo(successResultDescription.Name));
    }
}