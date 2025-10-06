// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Moryx.Tools;
using NUnit.Framework;

namespace Moryx.Tests.Extensions
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

            Assert.That(result.Length, Is.EqualTo(5), "Unexpected number of classes.");

            Assert.That(result, Does.Contain(typeof(BaseClass)));
            Assert.That(result, Does.Contain(typeof(ChildClass1)));
            Assert.That(result, Does.Contain(typeof(ChildClass2)));
            Assert.That(result, Does.Contain(typeof(GranChildClass1)));
            Assert.That(result, Does.Contain(typeof(GranChildClass2)));
        }

        [Test]
        public void ChildClass1Test()
        {
            Type[] result = ReflectionTool.GetPublicClasses<ChildClass1>();

            Assert.That(result.Length, Is.EqualTo(3), "Unexpected number of classes.");

            Assert.That(result, Does.Contain(typeof(ChildClass1)));
            Assert.That(result, Does.Contain(typeof(GranChildClass1)));
            Assert.That(result, Does.Contain(typeof(GranChildClass2)));
        }

        [TestCase(typeof(ChildClass2))]
        [TestCase(typeof(GranChildClass1))]
        [TestCase(typeof(GranChildClass2))]
        public void SingleClassTest(Type type)
        {
            Type[] result = ReflectionTool.GetPublicClasses(type);

            Assert.That(result.Length, Is.EqualTo(1), "Unexpected number of classes.");

            Assert.That(result, Does.Contain(type));
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
            Assert.That(result1, Is.Not.Null);
            Assert.That(result2, Is.Not.Null);
            Assert.That(result1, Is.InstanceOf<BaseClass>());
        }

        [TestCase(true, Description = "Create TypeChecker including derived types")]
        [TestCase(false, Description = "CreateTypeChecker compares types directly")]
        public void TypeChecker(bool derivedTypes)
        {
            // Arrange
            var matchObject = new ChildClass1();
            var noMatch = new ChildClass2();
            var sameTypeCheck = ReflectionTool.TypePredicate(typeof(ChildClass1), derivedTypes);
            var derivedTypeCheck = ReflectionTool.TypePredicate(typeof(BaseClass), derivedTypes);

            // Act
            var isSame = sameTypeCheck(matchObject);
            var different = sameTypeCheck(noMatch);
            var isDerived = derivedTypeCheck(matchObject);

            // Assert
            Assert.That(isSame, "Comparison for the same type must always return true");
            Assert.That(different, Is.False, "Comparison for incompatible types must return false");
            Assert.That(isDerived, Is.EqualTo(derivedTypes), "Comparison for derived types should only work if configured that way");
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
            Assert.That(child0.Foo, Is.EqualTo(42));
            Assert.That(child1.Foo, Is.EqualTo(42));
            Assert.That(child2.Foo, Is.EqualTo(42));
            Assert.That(child3.Foo, Is.EqualTo(42));
            Assert.That(child4.Foo, Is.EqualTo(42));
            Assert.That(child5.Foo, Is.EqualTo(42));
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
            Assert.That(child2.Foo, Is.EqualTo(child1.Foo));
            Assert.That(accesor, Is.LessThan(reflection), "Accessor should be faster");
        }

        [Test(Description = "Retreive all references of a certain type")]
        public void GetAllReferences()
        {
            // Arrange
            var target = new ReferenceClass
            {
                Ignore = 42,
                NotRelevant = "None",
                BaseRef1 = new BaseClass(),
                ChildRef = new ChildClass2(),
                Children1 = [],
                Children2 = new List<ChildClass2>(),
                EmptyArray = [],
                NullArray = null
            };

            // Act
            var references = ReflectionTool.GetReferences<BaseClass>(target).ToArray();

            // Assert
            Assert.That(references.Length, Is.EqualTo(6), "Tool did not detect all references");
        }

        [Test(Description = "Return the object values of the properties")]
        public void ReferenceContent()
        {
            // Arrange
            var target = new ReferenceClass
            {
                ChildRef = new ChildClass2(),
                Children2 = new List<ChildClass2> { new ChildClass2(), new ChildClass2() }
            };

            // Act
            var references = ReflectionTool.GetReferences<ChildClass2>(target).ToArray();

            // Assert
            Assert.That(references.Length, Is.EqualTo(2), "Tool did not detect all references");
            Assert.That(references[0].First(), Is.EqualTo(target.ChildRef), "Tool did not return the reference");
            Assert.That(target.Children2.First(), Is.EqualTo(references.Skip(1).First().First()), "Tool did not return the reference");
            Assert.That(target.Children2.Skip(1).First(), Is.EqualTo(references.Skip(1).First().Skip(1).First()), "Tool did not return the reference");
        }

        [Test(Description = "ReflectionTool must handle null or empty properties")]
        public void ReferencesHandlesNullOrEmpty()
        {
            // Arrange
            var target = new ReferenceClass
            {

                EmptyArray = [],
                NullArray = null
            };

            // Act
            var references = ReflectionTool.GetReferences<ChildClass1>(target).ToArray();
            var nullRef = ReflectionTool.GetReferences<ChildClass2>(target).ToArray();

            // Assert
            Assert.That(references.Length, Is.EqualTo(3), "Tool did not detect all references");
            Assert.That(references[0].Count(), Is.EqualTo(0));
            Assert.That(references[1].Count(), Is.EqualTo(0));
            Assert.That(references[2].Count(), Is.EqualTo(0));
            Assert.That(nullRef.Length, Is.EqualTo(2));
            Assert.That(nullRef[0].Key.Name, Is.EqualTo(nameof(ReferenceClass.ChildRef)));
            Assert.That(nullRef[0], Is.Empty);
        }

        [Test(Description = "ReflectionTool must handle null or empty properties")]
        public void FilterReferencesWithPredicate()
        {
            // Arrange
            var target = new ReferenceClass
            {
                Ignore = 42,
                NotRelevant = "None",
                BaseRef1 = new BaseClass(),
                ChildRef = new ChildClass2(),
                Children1 = [],
                Children2 = new List<ChildClass2>(),
                EmptyArray = [],
                NullArray = null
            };

            // Act
            var references = ReflectionTool.GetReferences<BaseClass>(target, p => p.Name.StartsWith("Child")).ToArray();

            // Assert
            Assert.That(references.Length, Is.EqualTo(3), "Tool did not detect all references");
        }
    }
}
