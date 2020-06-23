// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Diagnostics;
using System.Reflection;

namespace Moryx.Tools
{
    /// <summary>
    /// Class und method calling this method.
    /// </summary>
    public class Caller
    {
        /// <summary>
        /// Class calling this method
        /// </summary>
        public Type CallingClass { get; set; }
        /// <summary>
        /// Get calling method
        /// </summary>
        public MethodBase CallingMethod { get; set; }
    }

    /// <summary>
    /// Contains the class and method calling this method.
    /// </summary>
    public static class CallerId
    {
        /// <summary>
        /// Get class und method calling this method
        /// </summary>
        /// <returns></returns>
        public static Caller GetCaller()
        {
            var stackFrame = new StackTrace().GetFrame(2);
            var method = stackFrame.GetMethod();
            var caller = new Caller()
            {
                CallingMethod = method,
                CallingClass = method.DeclaringType
            };
            return caller;
        }
    }
}
