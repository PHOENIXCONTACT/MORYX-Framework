using System.Reflection;
using Marvin.Bindings;
using NUnit.Framework;

namespace Marvin.PlatformTools.Tests.Bindings
{
    [TestFixture]
    public class ReflectionResolverTests
    {
        [TestCase(false, Description = "Checks if the correct property will be resolved.")]
        [TestCase(true, Description = "Checks if the correct property will be resolved on a derived type.")]
        public void SimpleResolve(bool createDerived)
        {
            const string str = "HelloWorld";

            // Assert
            var obj = createDerived ? new SomeHiddenPropertyClass() : new SomeClass();
            obj.SimpleString = str;
            var reflectionResolver = new ReflectionResolver(nameof(SomeClass.SimpleString));

            // Act
            var result = reflectionResolver.Resolve(obj);

            // Assert
            Assert.IsTrue(result is string);
            Assert.AreEqual(str, (string)result);
        }

        [Test(Description = "Checks if the reflection resolver returns null by resolving an unknown property")]
        public void NullByUnknownProperty()
        {
            // Arrange
            var obj = new SomeClass();
            var reflectionResolver = new ReflectionResolver("SomeUnknownProperty");

            // Act
            var result = reflectionResolver.Resolve(obj);

            // Assert
            Assert.IsNull(result);
        }

        [Test(Description = "The error was that the reflection resolver throws an " +
                            nameof(AmbiguousMatchException) + "if a property was hidden by the new keyword.")]
        public void HiddenPropertyByNewKeyword()
        {
            // Arrange
            var reflectionResolver = new ReflectionResolver(nameof(SomeClass.SomeObject));
            var obj = new SomeHiddenPropertyClass
            {
                SomeObject = new SomeImplementation()
            };

            // Act
            // Should resolve the value of SomeObject
            object result = null;
            Assert.DoesNotThrow(delegate
            {
                result = reflectionResolver.Resolve(obj);
            });

            // Assert
            Assert.NotNull(result);
        }

        private interface ISomeInterface
        {

        }

        private class SomeImplementation : ISomeInterface
        {

        }

        private class SomeClass
        {
            public string SimpleString { get; set; }

            public ISomeInterface SomeObject { get; set; }
        }

        private class SomeHiddenPropertyClass : SomeClass
        {
            public new SomeImplementation SomeObject
            {
                get { return (SomeImplementation) base.SomeObject; }
                set { base.SomeObject = value; }
            }
        }
    }
}
