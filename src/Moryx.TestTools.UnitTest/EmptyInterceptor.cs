// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Castle.DynamicProxy;

namespace Marvin.TestTools.UnitTest
{
    /// <summary>
    /// Mock to use for castle. With this it is possible to create an emtpy implementation.
    /// Usage example: Register(Component.For(left anlge bracket)EmptyInterceptor(right angle bracket)());  
    /// Used for testing. DO NOT DELETE. If you do, inform Thomas and get your punishment.
    /// </summary>
    public class EmptyInterceptor : IInterceptor
    {
        /// <summary>
        /// Creates an Interception.
        /// </summary>
        /// <param name="invocation">An invocation.</param>
        public void Intercept(IInvocation invocation)
        {
        }
    }
}
