// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Tools
{
    /// <summary>
    /// Generic type that allows functions to always return a proper result,
    /// that either contains a valid value or an error and helps exercising
    /// error handling.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class FunctionResult<TResult>
    {
        /// <summary>
        /// Result value in case of success
        /// </summary>
        public TResult? Result { get; } = default;

        /// <summary>
        /// Error in case of failure
        /// </summary>
        public FunctionResultError? Error { get; } = null;

        /// <summary>
        /// Indicates if the result contains a valid value
        /// or not
        /// </summary>
        public bool Success => Error == null;

        /// <summary>
        /// Creates a result with a value
        /// </summary>
        /// <param name="result"></param>
        public FunctionResult(TResult result)
        {
            Result = result;
        }

        /// <summary>
        /// Creates an error result with <see cref="FunctionResultError"/>
        /// </summary>
        /// <param name="error"></param>
        public FunctionResult(FunctionResultError error)
        {
            Error = error;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Success
                ? Result?.ToString() ?? "null"
                : Error!.ToString();
        }

        /// <summary>
        /// Process result value and errors in a 'pattern matching '-like way
        /// </summary>
        /// <param name="success">Function to be excecuted in case of success</param>
        /// <param name="error">Function to be excecuted in case of an error</param>
        /// <returns><see cref="FunctionResult{TResult}" /> of the executed function</returns>
        public FunctionResult<TResult> Match(Func<TResult, FunctionResult<TResult>> success, Func<FunctionResultError, FunctionResult<TResult>> error)
            => Success ? success(Result!) : error(Error!);

        /// <summary>
        /// Process result value and errors in a 'pattern matching '-like way
        /// </summary>
        /// <param name="success">Action to be excecuted in case of success</param>
        /// <param name="error">Action to be excecuted in case of an error</param>
        /// <returns>The current <see cref="FunctionResult{TResult}" /></returns>
        public FunctionResult<TResult> Match(Action<TResult> success, Action<FunctionResultError> error)
            => Match(
                s =>
                {
                    success(s);
                    return this;
                },
                e =>
                {
                    error(e);
                    return this;
                });
    }

    /// <summary>
    /// <see cref="FunctionResult"/> of type <see cref="Nothing"/> to be used
    /// for functions that would return <see cref="void"/>
    /// </summary>
    public class FunctionResult : FunctionResult<Nothing>
    {
        /// <summary>
        /// Creates a `successful` <see cref="FunctionResult"/> with 'no' value
        /// <typeparam name="TResult"></typeparam>
        public FunctionResult() : base(new Nothing())
        {
        }


        /// <summary>
        /// Creates an error result with <see cref="FunctionResultError"/>
        /// </summary>
        /// <param name="error"></param>
        public FunctionResult(FunctionResultError error) : base(error)
        {
        }

        /// <summary>
        /// Helper to create an Ok <see cref="FunctionResult"/> in a descriptive way.
        /// </summary>
        /// <returns><see cref="FunctionResult"/></returns>
        public static FunctionResult Ok()
            => new();

        /// <summary>
        /// Helper to create a <see cref="FunctionResult"/> with an error message in a descriptive way.
        /// </summary>
        /// <returns><see cref="FunctionResult"/></returns>
        public static FunctionResult WithError(string message)
            => new(new FunctionResultError(message));

        /// <summary>
        /// Helper to create a <see cref="FunctionResult"/> with an <see cref="Exception"/> in a descriptive way.
        /// </summary>
        /// <returns><see cref="FunctionResult"/></returns>
        public static FunctionResult WithError(Exception exception)
            => new(new FunctionResultError(exception));

        /// <summary>
        /// Helper to create an Ok <see cref="FunctionResult"/> in a descriptive way.
        /// </summary>
        /// <returns><see cref="FunctionResult"/> of <see cref="TResult"/></returns>
        public static FunctionResult<TResult> Ok<TResult>(TResult result)
            => new(result);

        /// <summary>
        /// Helper to create a <see cref="FunctionResult{TResult}"/> with an error message in a descriptive way.
        /// </summary>
        /// <returns><see cref="FunctionResult{TResult}"/></returns>
        public static FunctionResult<TResult> WithError<TResult>(string message)
            => new(new FunctionResultError(message));

        /// <summary>
        /// Helper to create an Ok <see cref="FunctionResult"/> in a descriptive way.
        /// </summary>
        /// <returns><see cref="FunctionResult{TResult}"/></returns>
        public static FunctionResult<TResult> WithError<TResult>(Exception exception)
            => new(new FunctionResultError(exception));

    }

    /// <summary>
    /// Holds a description of the error and optionally an
    /// <see cref="Exception"/>
    public class FunctionResultError
    {
        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Exception that might be the reason for the error
        /// </summary>
        public Exception? Exception { get; } = null;


        /// <summary>
        /// Creates an error with error message
        /// </summary>
        /// <exception cref="ArgumentNullException">In case of <paramref name="message"/> is null</exception>
        public FunctionResultError(string message)
        {
            ArgumentNullException.ThrowIfNull(message);

            Message = message;
        }

        /// <summary>
        /// Creates an error with error message
        /// </summary>
        /// <exception cref="ArgumentNullException">In case of <paramref name="exception"/> is null</exception>
        public FunctionResultError(Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            Message = exception.Message;
            Exception = exception;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Exception != null
                ? Exception.Message
                : Message;
        }

    }


    /// <summary>
    /// Placeholder type to return nothing when for example <see cref="void"/>
    /// would be returned
    /// </summary>
    public class Nothing
    {
    }

    /// <summary>
    /// Extensions for <see cref="FunctionResult{TResult}"/>
    /// </summary>
    public static class FunctionResultExtensions
    {
        /// <summary>
        /// Executes the provided function in case of a successful result
        /// </summary>
        /// <returns><see cref=" FunctionResult{TResult}"/> returned by <paramref name="func"/></returns>
        public static FunctionResult<TResult> Then<TResult>(this FunctionResult<TResult> result, Func<TResult, FunctionResult<TResult>> func)
            => result.Match(func, _ => result);

        /// <summary>
        /// Executes the provided function in case of an error result
        /// </summary>
        /// <returns><see cref=" FunctionResult{TResult}"/> returned by <paramref name="func"/></returns>
        public static FunctionResult<TResult> Catch<TResult>(this FunctionResult<TResult> result, Func<FunctionResultError, FunctionResult<TResult>> func)
            => result.Match(_ => result, func);

        /// <summary>
        /// Executes the provided action in case of a successful result
        /// </summary>
        /// <returns>The underlying <see cref="FunctionResult{TResult}"/></returns>
        public static FunctionResult<TResult> Then<TResult>(this FunctionResult<TResult> result, Action<TResult> action)
            => result.Match(action, _ => { });

        /// <summary>
        /// Executes the provided action in case of a error result
        /// </summary>
        /// <returns>The underlying <see cref="FunctionResult{TResult}"/></returns>
        public static FunctionResult<TResult> Catch<TResult>(this FunctionResult<TResult> result, Action<FunctionResultError> action)
            => result.Match(_ => { }, action);
    }
}
