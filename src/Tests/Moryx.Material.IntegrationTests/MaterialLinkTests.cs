// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.Linking;
using NUnit.Framework;

namespace Moryx.Material.IntegrationTests;

[TestFixture]
internal sealed class MaterialLinkTests
{
    private LinkedTestContainer _container = null!;
    private TestHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _container = new LinkedTestContainer { Id = 1, Name = "Test Container" };
        _handler = new TestHandler();
    }

    [Test]
    public void Links_OfContainerDeclaringTwoLinks_ContainsBothLinks()
    {
        // Assert
        Assert.That(_container.Links, Is.EquivalentTo(new MaterialContainerLink[] { _container.First, _container.Second }));
    }

    [Test]
    public async Task AttachAsync_WithPendingRequest_AppliesRequestThroughHandler()
    {
        // Arrange
        await _container.First.LinkAsync("A");

        // Act
        var result = await _container.First.AttachAsync(_container, _handler);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result!.Status, Is.EqualTo(LinkingStatus.Applied));
            Assert.That(_container.First.HasPendingRequest, Is.False);
            Assert.That(_container.First.Reference, Is.TypeOf<ManagedTestReference>());
            Assert.That(_container.First.Reference!.Key, Is.EqualTo("A"));
            Assert.That(_handler.Calls, Is.EqualTo(["validate", "applied"]));
        });
    }

    [Test]
    public async Task AttachAsync_WithoutPendingRequest_ReturnsNoResult()
    {
        // Act
        var result = await _container.First.AttachAsync(_container, _handler);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Null);
            Assert.That(_container.First.IsAttached, Is.True);
            Assert.That(_handler.Calls, Is.Empty);
        });
    }

    [Test]
    public async Task LinkAsync_WithoutAttachedHandler_CompletesSynchronouslyAsPendingRequest()
    {
        // Act
        var task = _container.First.LinkAsync("A");

        // Assert
        Assert.That(task.IsCompleted, Is.True, "Resource constructors do not await the request, so it must not yield.");
        var result = await task;
        Assert.Multiple(() =>
        {
            Assert.That(result.Status, Is.EqualTo(LinkingStatus.Pending));
            Assert.That(_container.First.HasPendingRequest, Is.True);
            Assert.That(_container.First.PendingTarget!.Key, Is.EqualTo("A"));
            Assert.That(_container.First.Reference, Is.Null);
            Assert.That(_handler.Calls, Is.Empty);
        });
    }

    [Test]
    public async Task LinkAsync_WhenAttached_AppliesReferenceBeforeAppliedPhase()
    {
        // Arrange
        await _container.First.AttachAsync(_container, _handler);

        // Act
        var result = await _container.First.LinkAsync("A");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Status, Is.EqualTo(LinkingStatus.Applied));
            Assert.That(_handler.ReferenceDuringAppliedPhase!.Key, Is.EqualTo("A"));
            Assert.That(_container.Second.Reference, Is.Null, "Links of the same container must be independent.");
        });
    }

    [Test]
    public async Task LinkAsync_WhenRejected_KeepsLinkedReference()
    {
        // Arrange
        await _container.First.AttachAsync(_container, _handler);
        await _container.First.LinkAsync("A");
        _handler.Validate = _ =>
        {
            var context = new ValidationContext();
            context.AddError("Blocked");
            return Task.FromResult(new LinkingResponse<TestReference>(context));
        };

        // Act
        var result = await _container.First.LinkAsync("B");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Status, Is.EqualTo(LinkingStatus.Rejected));
            Assert.That(result.Context.HasErrors, Is.True);
            Assert.That(_container.First.Reference!.Key, Is.EqualTo("A"));
            Assert.That(_handler.Calls.Count(c => c == "applied"), Is.EqualTo(1));
        });
    }

    [Test]
    public async Task LinkAsync_WhenAppliedPhaseThrows_KeepsLinkAndReportsWarning()
    {
        // Arrange
        await _container.First.AttachAsync(_container, _handler);
        _handler.AppliedException = new InvalidOperationException("Lineage not available");

        // Act
        var result = await _container.First.LinkAsync("A");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Status, Is.EqualTo(LinkingStatus.Applied));
            Assert.That(_container.First.Reference!.Key, Is.EqualTo("A"));
            Assert.That(result.Context.Entries.Any(e => e.Severity == ValidationSeverity.Warning), Is.True);
        });
    }

    [Test]
    public async Task LinkAsync_WithConcurrentRequest_ProcessesRequestsSequentially()
    {
        // Arrange
        await _container.First.AttachAsync(_container, _handler);
        var gate = new TaskCompletionSource();
        _handler.Validate = async request =>
        {
            if (request.Target!.Key == "A")
            {
                await gate.Task;
            }
            return new LinkingResponse<TestReference>(new ValidationContext(), new ManagedTestReference(request.Target.Key));
        };

        // Act
        var first = _container.First.LinkAsync("A");
        var second = _container.First.LinkAsync("B");
        var validationsWhileBlocked = _handler.Calls.Count(c => c == "validate");
        gate.SetResult();
        await Task.WhenAll(first, second);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(validationsWhileBlocked, Is.EqualTo(1));
            Assert.That(_container.First.Reference!.Key, Is.EqualTo("B"));
            Assert.That(_handler.Requests[1].Previous!.Key, Is.EqualTo("A"));
        });
    }

    [Test]
    public async Task UnlinkAsync_WithPendingLinkRequest_WithdrawsRequest()
    {
        // Arrange
        await _container.First.LinkAsync("A");

        // Act
        var result = await _container.First.UnlinkAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Status, Is.EqualTo(LinkingStatus.Applied));
            Assert.That(_container.First.HasPendingRequest, Is.False);
        });
    }

    [Test]
    public async Task UnlinkAsync_WithoutLink_IsUnchanged()
    {
        // Act
        var result = await _container.First.UnlinkAsync();

        // Assert
        Assert.That(result.Status, Is.EqualTo(LinkingStatus.Unchanged));
    }

    [Test]
    public async Task UnlinkAsync_WithoutAttachedHandler_StoresPendingUnlink()
    {
        // Arrange
        await _container.First.AttachAsync(_container, _handler);
        await _container.First.LinkAsync("A");
        _container.First.Detach(_handler);

        // Act
        var result = await _container.First.UnlinkAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Status, Is.EqualTo(LinkingStatus.Pending));
            Assert.That(_container.First.IsUnlinkPending, Is.True);
            Assert.That(_container.First.Reference!.Key, Is.EqualTo("A"));
        });
    }

    [Test]
    public async Task Detach_OfOtherHandler_KeepsAttachedHandler()
    {
        // Arrange
        await _container.First.AttachAsync(_container, _handler);

        // Act
        _container.First.Detach(new TestHandler());

        // Assert
        Assert.That(_container.First.IsAttached, Is.True);
    }

    [Test]
    public async Task Substitute_WithReferenceToOtherBusinessObject_Throws()
    {
        // Arrange
        await _container.First.AttachAsync(_container, _handler);
        await _container.First.LinkAsync("A");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _container.First.Substitute(new TestReference("B")));
    }

    private sealed class TestHandler : ILinkHandler<TestReference>
    {
        public List<string> Calls { get; } = [];

        public List<LinkingRequest<TestReference>> Requests { get; } = [];

        public Func<LinkingRequest<TestReference>, Task<LinkingResponse<TestReference>>>? Validate { get; set; }

        public Exception? AppliedException { get; set; }

        public TestReference? ReferenceDuringAppliedPhase { get; private set; }

        public Task<LinkingResponse<TestReference>> ValidateAsync(IMaterialContainer container, LinkingRequest<TestReference> request,
            CancellationToken cancellationToken)
        {
            Calls.Add("validate");
            Requests.Add(request);
            return Validate?.Invoke(request) ?? Task.FromResult(new LinkingResponse<TestReference>(new ValidationContext(),
                request.Target is null ? null : new ManagedTestReference(request.Target.Key)));
        }

        public Task OnAppliedAsync(IMaterialContainer container, LinkingRequest<TestReference> request,
            LinkingResponse<TestReference> response, CancellationToken cancellationToken)
        {
            Calls.Add("applied");
            ReferenceDuringAppliedPhase = ((LinkedTestContainer)container).First.Reference;
            return AppliedException is null ? Task.CompletedTask : Task.FromException(AppliedException);
        }
    }
}
