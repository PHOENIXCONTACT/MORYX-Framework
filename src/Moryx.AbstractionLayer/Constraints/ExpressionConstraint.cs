// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer
{
    /// <summary>
    /// Constraint that evalutes an expression and compares it to an expected value
    /// </summary>
    public static class ExpressionConstraint
    {
        /// <summary>
        /// Create a constraint that checks if an expression equals the compare value
        /// </summary>
        public static IConstraint Equals<TContext>(Func<TContext, object> expression, object compareValue)
            where TContext : class, IConstraintContext
        {
            return new ExpressionEqualsConstraint<TContext>(expression, compareValue);
        }

        /// <summary>
        /// Create a constraint, that checks if an expression is less or equal to the return value
        /// </summary>
        public static IConstraint LessOrEqual<TContext>(Func<TContext, IComparable> expression, IComparable compareValue)
            where TContext : class, IConstraintContext
        {
            return new ExpressionLessConstraint<TContext>(expression, compareValue);
        }

        /// <summary>
        /// Create a constraint, that checks if an expression is greater or equal to the return value
        /// </summary>
        public static IConstraint GreaterOrEqual<TContext>(Func<TContext, IComparable> expression, IComparable compareValue)
            where TContext : class, IConstraintContext
        {
            return new ExpressionGreaterConstraint<TContext>(expression, compareValue);
        }

        /// <summary>
        /// Nested generic base class for the differen types of expression constraints
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        private abstract class ExpressionConstraintBase<TContext> : IConstraint
            where TContext : class, IConstraintContext
        {
            /// <summary>
            /// Value the expressions return value is compared to
            /// </summary>
            protected object Value { get; }

            /// <summary>
            /// Expression to retrieve the value
            /// </summary>
            protected Func<TContext, object> Expression { get; }

            /// <summary>
            /// Create a new expression constraint
            /// </summary>
            protected ExpressionConstraintBase(Func<TContext, object> expression, object value)
            {
                Expression = expression;
                Value = value;
            }

            /// <summary>
            /// Delegate constraint comparison to the derived type
            /// </summary>
            public bool Check(IConstraintContext context)
            {
                var typedContext = context as TContext;
                return typedContext != null && CheckConstraint(typedContext);
            }

            /// <summary>
            /// Typed constraint check
            /// </summary>
            protected abstract bool CheckConstraint(TContext context);
        }

        /// <summary>
        /// Check if the expressions return value equals the <see cref="ExpressionConstraintBase{TContext}.Value"/>
        /// </summary>
        private class ExpressionEqualsConstraint<TContext> : ExpressionConstraintBase<TContext>
            where TContext : class, IConstraintContext
        {
            public ExpressionEqualsConstraint(Func<TContext, object> expression, object value) : base(expression, value)
            {
            }

            protected override bool CheckConstraint(TContext context)
            {
                return Expression(context).Equals(Value);
            }
        }

        /// <summary>
        /// Check if the expression return value is less or equal to <see cref="ExpressionConstraintBase{TContext}.Value"/>
        /// </summary>
        private class ExpressionLessConstraint<TContext> : ExpressionConstraintBase<TContext>
            where TContext : class, IConstraintContext
        {
            public ExpressionLessConstraint(Func<TContext, object> expression, object value) : base(expression, value)
            {
            }

            protected override bool CheckConstraint(TContext context)
            {
                var compareValue = (IComparable)Value;
                var returnValue = (IComparable)Expression(context);
                return returnValue.CompareTo(compareValue) <= 0;
            }
        }

        /// <summary>
        /// Check if the expression return value is greater or equal to <see cref="ExpressionConstraintBase{TContext}.Value"/>
        /// </summary>
        private class ExpressionGreaterConstraint<TContext> : ExpressionConstraintBase<TContext>
            where TContext : class, IConstraintContext
        {
            public ExpressionGreaterConstraint(Func<TContext, object> expression, object value) : base(expression, value)
            {
            }

            protected override bool CheckConstraint(TContext context)
            {
                var compareValue = (IComparable)Value;
                var returnValue = (IComparable)Expression(context);
                return returnValue.CompareTo(compareValue) >= 0;
            }
        }
    }
}
