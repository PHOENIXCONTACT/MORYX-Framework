using System;
using System.Diagnostics;
using System.Linq;
using Marvin.Tools;
using NUnit.Framework;

namespace Marvin.Tests.Extensions
{
    [TestFixture]
    public class ReflectionToolTests
    {
        [SetUp]
        public void SetDirectory()
        {
            ReflectionTool.TestMode = true;
        }

        [TestCase(typeof(IBaseInterface))]
        [TestCase(typeof(ITestInterface))]
        [TestCase(typeof(AbstractBaseClass))]
        [TestCase(typeof(BaseClass))]
        public void BasicTest(Type type)
        {
            Type[] result = ReflectionTool.GetPublicClasses(type);

            Assert.AreEqual(5, result.Length, "Unexpected number of classes.");

            Assert.True(result.Contains(typeof(BaseClass)));
            Assert.True(result.Contains(typeof(ChildClass1)));
            Assert.True(result.Contains(typeof(ChildClass2)));
            Assert.True(result.Contains(typeof(GranChildClass1)));
            Assert.True(result.Contains(typeof(GranChildClass2)));
        }

        [Test]
        public void ChildClass1Test()
        {
            Type[] result = ReflectionTool.GetPublicClasses<ChildClass1>();

            Assert.AreEqual(3, result.Length, "Unexpected number of classes.");

            Assert.True(result.Contains(typeof(ChildClass1)));
            Assert.True(result.Contains(typeof(GranChildClass1)));
            Assert.True(result.Contains(typeof(GranChildClass2)));
        }

        [TestCase(typeof(ChildClass2))]
        [TestCase(typeof(GranChildClass1))]
        [TestCase(typeof(GranChildClass2))]
        public void SingleClassTest(Type type)
        {
            Type[] result = ReflectionTool.GetPublicClasses(type);

            Assert.AreEqual(1, result.Length, "Unexpected number of classes.");

            Assert.True(result.Contains(type));
        }

        [TestCase(typeof(BaseClass), Description = "Create constructor for the base type")]
        [TestCase(typeof(ChildClass1), Description = "Create constructor for derived type")]
        public void CreateConstructor(Type targetType)
        {
            // Arrange
            var target = typeof(BaseClass);

            // Act
            var func1 = ReflectionTool.ConstructorDelegate(target);
            var func2 = ReflectionTool.ConstructorDelegate<BaseClass>(targetType);
            var result1 = func1();
            var result2 = func2();

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.IsInstanceOf<BaseClass>(result1);
        }

        [Test(Description = "Create accessor to read and write custom property")]
        public void PropertyAccessor()
        {
            // Arrange
            var childType = typeof(ChildClass1);
            var prop = childType.GetProperty(nameof(ChildClass1.Foo));

            // Act
            var accessor0 = ReflectionTool.PropertyAccessor(prop);
            var child0 = new ChildClass1();
            accessor0.WriteProperty(child0, 42);
            var accessor1 = ReflectionTool.PropertyAccessor<IBaseInterface>(prop);
            var child1 = new ChildClass1();
            accessor1.WriteProperty(child1, 42);
            var child2 = new ChildClass1();
            accessor1.WriteProperty(child2, 42L);
            var accessor2 = ReflectionTool.PropertyAccessor<BaseClass, long>(prop);
            var child3 = new ChildClass1();
            accessor2.WriteProperty(child3, 42L);
            var accessor3 = ReflectionTool.PropertyAccessor<BaseClass, short>(prop);
            var child4 = new ChildClass1();
            accessor3.WriteProperty(child4, 42);
            var accessor4 = ReflectionTool.PropertyAccessor<ChildClass1, int>(prop);
            var child5 = new ChildClass1();
            accessor4.WriteProperty(child5, 42);

            // Assert
            Assert.AreEqual(42, child0.Foo);
            Assert.AreEqual(42, child1.Foo);
            Assert.AreEqual(42, child2.Foo);
            Assert.AreEqual(42, child3.Foo);
            Assert.AreEqual(42, child4.Foo);
            Assert.AreEqual(42, child5.Foo);
        }

        //[Test]
        public void PropertyAccessorIsFaster()
        {
            // Arrange
            var childType = typeof(ChildClass1);
            var child1 = new ChildClass1();
            var child2 = new ChildClass1();
            var prop = childType.GetProperty(nameof(ChildClass1.Foo));
            var accessor = ReflectionTool.PropertyAccessor<BaseClass, int>(prop);
            // Run once for JIT
            var value = prop.GetValue(new ChildClass1());
            prop.SetValue(new ChildClass1(), 1);
            accessor.ReadProperty(new ChildClass1());
            accessor.WriteProperty(new ChildClass1(), 1);

            // Act
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            for (int i = 0; i < 100; i++)
            {
                var next = (int)prop.GetValue(child1) + i;
                prop.SetValue(child1, next);
            }
            stopWatch.Stop();
            var reflection = stopWatch.ElapsedTicks;
            stopWatch.Restart();
            for (int i = 0; i < 100; i++)
            {
                var next = accessor.ReadProperty(child2) + i;
                accessor.WriteProperty(child2, next);
            }
            stopWatch.Stop();
            var accesor = stopWatch.ElapsedTicks;

            // Asser
            Assert.AreEqual(child1.Foo, child2.Foo);
            Assert.Less(accesor, reflection, "Accessor should be faster");
        }
    }
}