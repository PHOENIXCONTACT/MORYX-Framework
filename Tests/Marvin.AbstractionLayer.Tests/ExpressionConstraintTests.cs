using NUnit.Framework;

namespace Marvin.AbstractionLayer.Tests
{
    [TestFixture]
    public class ExpressionConstraintTests
    {
        private class DummyContext : IConstraintContext
        {
            public int Foo { get; set; }
        }

        /// <summary>
        /// Check if the expression equals the compare value
        /// </summary>
        [TestCase(100, 100, Description = "Compare two matching values")]
        [TestCase(100, 42, Description = "Compare not matching values")]
        public void ExpressionEquals(int foo, int compareValue)
        {
            // Arrange:
            var context = new DummyContext
            {
                Foo = foo
            };
            var constraint = ExpressionConstraint.Equals<DummyContext>(c => c.Foo, compareValue);

            // Act
            var result = constraint.Check(context);

            // Arrange
            Assert.AreEqual(result, foo == compareValue, "Constraint failed to compare objects");
        }

        /// <summary>
        /// Check if the expression is less or equal the compare value
        /// </summary>
        [TestCase(100, 100, Description = "Compare two equal values")]
        [TestCase(100, 42, Description = "Value too big")]
        [TestCase(42, 100, Description = "Value is less")]
        public void ExpressionLess(int foo, int compareValue)
        {
            // Arrange:
            var context = new DummyContext
            {
                Foo = foo
            };
            var constraint = ExpressionConstraint.LessOrEqual<DummyContext>(c => c.Foo, compareValue);

            // Act
            var result = constraint.Check(context);

            // Arrange
            Assert.AreEqual(result, foo <= compareValue, "Constraint failed to compare objects");
        }

        /// <summary>
        /// Check if the expression is greater or equal the compare value
        /// </summary>
        [TestCase(100, 100, Description = "Compare two equal values")]
        [TestCase(100, 42, Description = "Value is bigger")]
        [TestCase(42, 100, Description = "Value is too small")]
        public void ExpressionGreater(int foo, int compareValue)
        {
            // Arrange:
            var context = new DummyContext
            {
                Foo = foo
            };
            var constraint = ExpressionConstraint.GreaterOrEqual<DummyContext>(c => c.Foo, compareValue);

            // Act
            var result = constraint.Check(context);

            // Arrange
            Assert.AreEqual(result, foo >= compareValue, "Constraint failed to compare objects");
        }
    }
}