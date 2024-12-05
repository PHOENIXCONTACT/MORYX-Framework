// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using NUnit.Framework;
using System;
using Moq;
using Moryx.Tools.FunctionResult;

namespace Moryx.Tests.Tools;

[TestFixture]
public class FunctionResultWithTypeTests : FunctionResultTestsBase
{
    protected Mock<Func<int, FunctionResult<int>>> _funcMockSuccess;
    protected Mock<Func<FunctionResultError, FunctionResult<int>>> _funcMockError;

    protected Mock<Action<int>> _actionMockSuccess;
    protected Mock<Action<FunctionResultError>> _actionMockError;

    [SetUp]
    public void Setup()
    {
        _funcMockSuccess = new Mock<Func<int, FunctionResult<int>>>();
        _funcMockSuccess
            .Setup(f => f(It.IsAny<int>()))
            .Returns((int arg) => new FunctionResult<int>(arg));
        _funcMockError = new Mock<Func<FunctionResultError, FunctionResult<int>>>();
        _funcMockError
            .Setup(f => f(It.IsAny<FunctionResultError>()))
            .Returns((FunctionResultError arg) => new FunctionResult<int>(arg));

        _actionMockSuccess = new Mock<Action<int>>();
        _actionMockError = new Mock<Action<FunctionResultError>>();
    }

    [Test]
    public void ResultWithValueGetsCreated()
    {
        var result = new FunctionResult<int>(1);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Error, Is.Null);
        Assert.That(result.Result, Is.EqualTo(1));
        Assert.That(result.ToString(), Is.EqualTo("1"));
    }

    [Test]
    public void ErrorResultWithMessageGetsCreated()
    {
        var result = new FunctionResult<int>(new FunctionResultError(Message));

        Assert.That(result.Success, Is.False);
        Assert.That(result.Result, Is.EqualTo(0));

        Assert.That(result.Error.Message, Is.EqualTo(Message));
        Assert.That(result.Error.Exception, Is.Null);
        Assert.That(result.ToString(), Is.EqualTo(Message));
    }

    [Test]
    public void ErrorResultWithExceptionGetsCreated()
    {
        var result = new FunctionResult<int>(new FunctionResultError(new Exception(ExceptionMessage)));

        Assert.That(result.Success, Is.False);
        Assert.That(result.Result, Is.EqualTo(0));

        Assert.That(result.Error.Message, Is.EqualTo(ExceptionMessage));
        Assert.That(result.Error.Exception, Is.TypeOf<Exception>());
        Assert.That(result.ToString(), Is.EqualTo(ExceptionMessage));
    }

    [Test]
    public void ResultWithValueGetsCreatedByUsingExtension()
    {
        var result = FunctionResult.Ok(10);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.EqualTo(10));

        Assert.That(result.Error, Is.Null);
        Assert.That(result.ToString(), Is.EqualTo("10"));
    }

    [Test]
    public void ErrorResultWithMessageGetsCreatedByUsingExtension()
    {
        var result = FunctionResult.WithError<int>(Message);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Result, Is.EqualTo(0));


        Assert.That(result.Error.Message, Is.EqualTo(Message));
        Assert.That(result.Error.Exception, Is.Null);
        Assert.That(result.ToString(), Is.EqualTo(Message));
    }

    [Test]
    public void ErrorResultWithExceptionGetsCreatedByUsingExtension()
    {
        var result = FunctionResult.WithError<int>(new Exception(ExceptionMessage));

        Assert.That(result.Success, Is.False);
        Assert.That(result.Result, Is.EqualTo(0));

        Assert.That(result.Error.Message, Is.EqualTo(ExceptionMessage));
        Assert.That(result.Error.Exception, Is.TypeOf<Exception>());
        Assert.That(result.ToString(), Is.EqualTo(ExceptionMessage));
    }

    [Test]
    public void ResultToStringEqualsTheResultsToStringReturnValue()
    {
        var floatResult = FunctionResult.Ok(3.14f);
        string floatAsString = Convert.ToString(3.14f); // avoid localization issues
        var noResult = FunctionResult.Ok(new Nothing());
        var nullResult = FunctionResult.Ok<object>(null);

        Assert.Multiple(() =>
        {
            Assert.That($"{floatResult}", Is.EqualTo(floatAsString));
            Assert.That($"{noResult}", Is.EqualTo(new Nothing().ToString()));
            Assert.That($"{nullResult}", Is.EqualTo("null"));
        });
    }

    [Test]
    public void CannotCreateErrorResultWithNullExeption()
    {
        Assert.Throws<ArgumentNullException>(() => { FunctionResult.WithError<int>((Exception)null); });
    }

    [Test]
    public void CannotCreateErrorResultWithNullMessage()
    {
        Assert.Throws<ArgumentNullException>(() => { FunctionResult.WithError<int>((string)null); });
    }

    [Test]
    public void SuccessResultMatchesSuccess()
    {
        var result = FunctionResult.Ok(200);

        var matchResult = result
            .Match(
                success: _funcMockSuccess.Object,
                error: _funcMockError.Object
            );

        _funcMockSuccess.Verify(f => f(It.IsAny<int>()), Times.Once);
        _funcMockError.Verify(f => f(It.IsAny<FunctionResultError>()), Times.Never);

        Assert.That(result, Is.Not.SameAs(matchResult));
    }


    [Test]
    public void ErrorResultMatchesErrorWithException()
    {
        var result = FunctionResult.WithError<int>(new Exception("Internal server error"));

        var matchResult = result
            .Match(
                success: _funcMockSuccess.Object,
                error: _funcMockError.Object
            );

        _funcMockSuccess.Verify(f => f(It.IsAny<int>()), Times.Never);
        _funcMockError.Verify(f => f(It.IsAny<FunctionResultError>()), Times.Once);

        // The assertion verifies, that the result can be different from the original
        Assert.That(result, Is.Not.SameAs(matchResult));
    }

    [Test]
    public void ErrorResultMatchesErrorWithMessage()
    {
        var result = FunctionResult.WithError<int>("500");

        var matchResult = result
            .Match(
                success: _funcMockSuccess.Object,
                error: _funcMockError.Object
            );

        _funcMockSuccess.Verify(f => f(It.IsAny<int>()), Times.Never);
        _funcMockError.Verify(f => f(It.IsAny<FunctionResultError>()), Times.Once);

        // The assertion verifies, that the result can be different from the original
        Assert.That(result, Is.Not.SameAs(matchResult));
    }

    [Test]
    public void SuccessResultMatchesSuccessAction()
    {
        var result = FunctionResult.Ok(200);

        var matchResult = result
             .Match(
                success: _actionMockSuccess.Object,
                error: _actionMockError.Object
            );

        _actionMockSuccess.Verify(a => a(It.IsAny<int>()), Times.Once);
        _actionMockError.Verify(a => a(It.IsAny<FunctionResultError>()), Times.Never);

        Assert.That(result, Is.SameAs(matchResult));
    }

    [Test]
    public void ExceptionResultMatchesErrorAction()
    {
        var result = FunctionResult.WithError<int>(new Exception("Internal server error"));

        var matchResult = result
            .Match(
                success: _actionMockSuccess.Object,
                error: _actionMockError.Object
            );

        _actionMockSuccess.Verify(a => a(It.IsAny<int>()), Times.Never);
        _actionMockError.Verify(a => a(It.IsAny<FunctionResultError>()), Times.Once);

        Assert.That(result, Is.SameAs(matchResult));
    }

    [Test]
    public void ErrorMessageResultMatchesErrorAction()
    {
        var result = FunctionResult.WithError<int>("500");

        var matchResult = result
            .Match(
                success: _actionMockSuccess.Object,
                error: _actionMockError.Object
            );

        _actionMockSuccess.Verify(a => a(It.IsAny<int>()), Times.Never);
        _actionMockError.Verify(a => a(It.IsAny<FunctionResultError>()), Times.Once);

        Assert.That(result, Is.SameAs(matchResult));
    }

    [Test]
    public void ExecutesFuncOnError()
    {
        FunctionResult.WithError<int>("500")
            .Then(_funcMockSuccess.Object)
            .Catch(_funcMockError.Object)
            ;

        _funcMockSuccess.Verify(f => f(It.IsAny<int>()), Times.Never);
        _funcMockError.Verify(f => f(It.IsAny<FunctionResultError>()), Times.Once);
    }

    [Test]
    public void ExecutesFuncOnSuccess()
    {
        FunctionResult.Ok(201)
            .Catch(_funcMockError.Object)
            .Then(_funcMockSuccess.Object)
            ;

        _funcMockSuccess.Verify(f => f(It.IsAny<int>()), Times.Once);
        _funcMockError.Verify(f => f(It.IsAny<FunctionResultError>()), Times.Never);
    }

    [Test]
    public void ExecutesActionOnError()
    {
        FunctionResult.WithError<int>("500")
            .Then(_actionMockSuccess.Object)
            .Catch(_actionMockError.Object)
            ;

        _actionMockSuccess.Verify(a => a(It.IsAny<int>()), Times.Never);
        _actionMockError.Verify(a => a(It.IsAny<FunctionResultError>()), Times.Once);
    }

    [Test]
    public void ExecutesActionOnSuccess()
    {
        FunctionResult.Ok(42)
             .Catch(_actionMockError.Object)
             .Then(_actionMockSuccess.Object)
             ;

        _actionMockSuccess.Verify(a => a(It.IsAny<int>()), Times.Once);
        _actionMockError.Verify(a => a(It.IsAny<FunctionResultError>()), Times.Never);
    }

}
