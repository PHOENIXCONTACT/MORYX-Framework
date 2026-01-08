// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Workplans;
using NUnit.Framework;

namespace Moryx.Tests.Workplans;

[TestFixture]
public class PlacesTests
{
    [TestCase(false, Description = "Add token in running state")]
    [TestCase(true, Description = "Add token in paused state")]
    public void AddToken(bool paused)
    {
        // Arrange
        var place = new Place();
        var dummy = new DummyToken();
        IToken raised = null;
        place.TokenAdded += (sender, token) => raised = token;

        // Act
        if (paused)
            place.Pause();
        place.Add(dummy);

        // Assert
        if (paused)
            Assert.That(raised, Is.Null);
        else
            Assert.That(raised, Is.EqualTo(dummy));
    }

    [Test]
    public void Resume()
    {
        // Arrange
        var place = new Place();
        var dummy = new DummyToken();
        IToken raised = null;
        place.TokenAdded += (sender, token) => raised = token;

        // Act
        place.Tokens = [dummy];
        place.Resume();

        // Assert
        Assert.That(raised, Is.EqualTo(dummy));
    }

    [TestCase(true, true, Description = "Existing token and event registered")]
    [TestCase(false, true, Description = "No token, but event registered")]
    [TestCase(true, false, Description = "Existing token, but no event registered")]
    [TestCase(false, false, Description = "No token and no event")]
    public void Remove(bool exisiting, bool eventRaised)
    {
        // Arrange
        var place = new Place();
        var dummy = new DummyToken();
        if (eventRaised)
            place.TokenRemoved += (sender, token) => { };

        // Act
        if (exisiting)
            place.Tokens = [dummy];
        place.Remove(dummy);
    }
}