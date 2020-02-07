// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Tools;

namespace Moryx.Runtime.Maintenance.Contracts
{
    /// <summary>
    /// Default response for simple executions
    /// </summary>
    public class InvocationResponse
    {
        /// <summary>
        /// Default constructor with a successful result
        /// </summary>
        public InvocationResponse()
        {
            Success = true;
        }

        /// <summary>
        /// Default constructor with a unsuccessful result based on an exception
        /// </summary>
        public InvocationResponse(Exception e)
        {
            Success = false;
            ExceptionType = e.GetType().ToString();
            ErrorMessage = ExceptionPrinter.Print(e);
        }

        /// <summary>
        /// Default constructor with a unsuccessful result based on a error message
        /// </summary>
        public InvocationResponse(string errorMessage)
        {
            Success = false;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// <c>true</c> if request was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Type of the exception
        /// </summary>
        public string ExceptionType { get; set; }

        /// <summary>
        /// Error message of the exception
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
