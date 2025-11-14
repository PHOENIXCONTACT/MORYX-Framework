// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moq;
using Moryx.Tools;
using NUnit.Framework;

namespace Moryx.Tests.Tools;

[TestFixture]
public class FunctionResultWithNothingTests : FunctionResultTestsBase
{
    protected Mock<Func<Nothing, FunctionResult<Nothing>>> _funcMockSuccess;
    protected Mock<Func<FunctionResultError, FunctionResult<Nothing>>> _funcMockError;

    protected Mock<Action<Nothing>> _actionMockSuccess;
    protected Mock<Action<FunctionResultError>> _actionMockError;

    [SetUp]
    public void Setup()
    {
        _funcMockSuccess = new Mock<Func<Nothing, FunctionResult<Nothing>>>();
        _funcMockSuccess
            .Setup(f => f(It.IsAny<Nothing>()))
            .Returns((Nothing arg) => new FunctionResult<Nothing>(arg));
        _funcMockError = new Mock<Func<FunctionResultError, FunctionResult<Nothing>>>();
        _funcMockError
            .Setup(f => f(It.IsAny<FunctionResultError>()))
            .Returns((FunctionResultError arg) => new FunctionResult<Nothing>(arg));

        _actionMockSuccess = new Mock<Action<Nothing>>();
        _actionMockError = new Mock<Action<FunctionResultError>>();
    }

    [Test]
    public void ResultWithValueGetCreated()
    {
        var result = new FunctionResult();

        Assert.That(result.Success, Is.True);
        Assert.That(result.Error, Is.Null);
        Assert.That(result.Result, Is.TypeOf<Nothing>());
        Assert.That(result.ToString(), Contains.Substring("Nothing"));
    }

    [Test]
    public void ErrorResultWithMessageGetsCreated()
    {
        var result = new FunctionResult(new FunctionResultError(Message));

        Assert.That(result.Success, Is.False);
        Assert.That(result.Result, Is.Null);

        Assert.That(Message, Is.EqualTo(result.Error.Message));
        Assert.That(result.Error.Exception, Is.Null);
        Assert.That(Message, Is.EqualTo(result.ToString()));
    }

    [Test]
    public void ErrorResultWithExceptionGetsCreated()
    {
        var result = new FunctionResult(new FunctionResultError(new Exception(ExceptionMessage)));

        Assert.That(result.Success, Is.False);
        Assert.That(result.Result, Is.Null);

        Assert.That(ExceptionMessage, Is.EqualTo(result.Error.Message));
        Assert.That(result.Error.Exception, Is.TypeOf<Exception>());
        Assert.That(ExceptionMessage, Is.EqualTo(result.ToString()));
    }

    [Test]
    public void ResultWithNothingGetsCreatedByUsingExtension()
    {
        var result = FunctionResult.Ok();

        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.TypeOf<Nothing>());
        Assert.That(result.Error, Is.Null);
        Assert.That(result.ToString(), Contains.Substring("Nothing"));
    }

    [Test]
    public void ErrorResultWithMessageGetsCreatedByUsingExtension()
    {
        var result = FunctionResult.WithError(Message);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Result, Is.Null);

        Assert.That(Message, Is.EqualTo(result.Error.Message));
        Assert.That(result.Error.Exception, Is.Null);
        Assert.That(Message, Is.EqualTo(result.ToString()));
    }

    [Test]
    public void ErrorResultWithExceptionGetsCreatedByUsingExtension()
    {
        FunctionResult result = FunctionResult.WithError(new Exception(ExceptionMessage));

        Assert.That(result.Success, Is.False);
        Assert.That(result.Result, Is.Null);

        Assert.That(ExceptionMessage, Is.EqualTo(result.Error.Message));
        Assert.That(result.Error.Exception, Is.TypeOf<Exception>());
        Assert.That(ExceptionMessage, Is.EqualTo(result.ToString()));
    }

    [Test]
    public void CannotCreateErrorResultWithNullExeption()
    {
        Assert.Throws<ArgumentNullException>(() => { FunctionResult.WithError((Exception)null); });
    }

    [Test]
    public void CannotCreateErrorResultWithNullMessage()
    {
        Assert.Throws<ArgumentNullException>(() => { FunctionResult.WithError((string)null); });
    }

    [Test]
    public void ExecutesFuncOnError()
    {
        FunctionResult.WithError("500")
            .Then(_funcMockSuccess.Object)
            .Catch(_funcMockError.Object)
            ;

        _funcMockSuccess.Verify(f => f(It.IsAny<Nothing>()), Times.Never);
        _funcMockError.Verify(f => f(It.IsAny<FunctionResultError>()), Times.Once);
    }

    [Test]
    public void ExecutesFuncOnSuccess()
    {
        FunctionResult.Ok()
            .Catch(_funcMockError.Object)
            .Then(_funcMockSuccess.Object)
            ;

        _funcMockSuccess.Verify(f => f(It.IsAny<Nothing>()), Times.Once);
        _funcMockError.Verify(f => f(It.IsAny<FunctionResultError>()), Times.Never);
    }

    [Test]
    public void ExecutesActionOnError()
    {
        FunctionResult.WithError("500")
            .Then(_actionMockSuccess.Object)
            .Catch(_actionMockError.Object)
            ;

        _actionMockSuccess.Verify(a => a(It.IsAny<Nothing>()), Times.Never);
        _actionMockError.Verify(a => a(It.IsAny<FunctionResultError>()), Times.Once);
    }

    [Test]
    public void ExecutesActionOnSuccess()
    {
        FunctionResult.Ok()
             .Catch(_actionMockError.Object)
             .Then(_actionMockSuccess.Object)
             ;

        _actionMockSuccess.Verify(a => a(It.IsAny<Nothing>()), Times.Once);
        _actionMockError.Verify(a => a(It.IsAny<FunctionResultError>()), Times.Never);
    }
}
