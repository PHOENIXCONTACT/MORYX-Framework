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

            // Arrange
            var obj = createDerived ? new SomeHiddenPropertyClass() : new SomeClass();
            obj.SimpleString = str;
            IBindingResolver reflectionResolver = new ReflectionResolver(nameof(SomeClass.SimpleString));

            // Act
            var result = reflectionResolver.Resolve(obj);

            // Assert
            Assert.IsTrue(result is string);
            Assert.AreEqual(str, (string)result);
        }

        [Test]
        public void SimpleAssign()
        {
            const string str = "HelloWorld";

            // Arrange
            var obj = new SomeHiddenPropertyClass();
            IBindingResolver reflectionResolver = new ReflectionResolver(nameof(SomeClass.SimpleString));

            // Act
            var result = reflectionResolver.Update(obj, str);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(str, obj.SimpleString);
        }

        [Test(Description = "Checks if the reflection resolver returns null by resolving an unknown property")]
        public void NullByUnknownProperty()
        {
            // Arrange
            var obj = new SomeClass();
            IBindingResolver reflectionResolver = new ReflectionResolver("SomeUnknownProperty");

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
            IBindingResolver reflectionResolver = new ReflectionResolver(nameof(SomeClass.SomeObject));
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

        
    }
}
