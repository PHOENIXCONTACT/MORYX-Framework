// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using System.Runtime.CompilerServices;

namespace Moryx.TestTools.NUnit
{
    public abstract class MAssert
    {
        public static void That(bool condition, string? message = null, [CallerArgumentExpression(nameof(condition))] string? predicateExpression = null)
        {
            That(condition, Is.True, message, predicateExpression);
        }
        public static void That(Func<bool> predicate, string? message = null, [CallerArgumentExpression(nameof(predicate))]string? predicateExpression = null)
        {
            That(predicate, Is.True, message, predicateExpression);
        }

        public static void That<T>(T actual, IResolveConstraint constraint, string? message = null, [CallerArgumentExpression(nameof(actual))] string? predicateExpression = null)
        {
            if (message != null)
            {
                message = $"{message}\nExpression: {predicateExpression}";
            }
            else
            {
                message = predicateExpression;
            }
            Assert.That(actual, constraint, message);
        }

        public static void That<T>(Func<T> actualExpression, Constraint constraint, string? message = null, [CallerArgumentExpression(nameof(actualExpression))] string? predicateExpression = null)
        {
            if (message != null)
            {
                message = $"{message}\nExpression: {predicateExpression}";
            }
            else
            {
                message = predicateExpression;
            }
            int fails = TestExecutionContext.CurrentContext.CurrentResult.PendingFailures;
            T value = default(T)!; 
            Assert.That<T>(() => value = actualExpression(), new ThrowsNothingConstraint(), $"{message}\nExpected {constraint.Description} and");
            if (TestExecutionContext.CurrentContext.CurrentResult.PendingFailures > fails) return; // TODO: Check if we there could be multithreading issues and whether or not we care
            Assert.That(value, constraint, message);
        }
    }

}

