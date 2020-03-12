// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
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
            Assert.IsTrue(isSame, "Comparison for the same type must always return true");
            Assert.IsFalse(different, "Comparison for incompatible types must return false");
            Assert.AreEqual(derivedTypes, isDerived, "Comparison for derived types should only work if configured that way");
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
                Children1 = new List<ChildClass1>(),
                Children2 = new List<ChildClass2>(),
                EmptyArray = new GranChildClass1[0],
                NullArray = null
            };

            // Act
            var references = ReflectionTool.GetReferences<BaseClass>(target).ToArray();

            // Assert
            Assert.AreEqual(6, references.Length, "Tool did not detect all references");
        }

        [Test(Description = "Return the object values of the properties")]
        public void ReferenceContent()
        {
            // Arrange
            var target = new ReferenceClass
            {
                ChildRef = new ChildClass2(),
                Children2 = new List<ChildClass2> { new ChildClass2(), new ChildClass2()}
            };

            // Act
            var references = ReflectionTool.GetReferences<ChildClass2>(target).ToArray();

            // Assert
            Assert.AreEqual(2, references.Length, "Tool did not detect all references");
            Assert.AreEqual(target.ChildRef, references[0].First(), "Tool did not return the reference");
            Assert.AreEqual(target.Children2.First(), references.Skip(1).First().First(), "Tool did not return the reference");
            Assert.AreEqual(target.Children2.Skip(1).First(), references.Skip(1).First().Skip(1).First(), "Tool did not return the reference");
        }

        [Test(Description = "ReflectionTool must handle null or empty properties")]
        public void ReferencesHandlesNullOrEmpty()
        {
            // Arrange
            var target = new ReferenceClass
            {
                EmptyArray = new GranChildClass1[0],
                NullArray = null
            };

            // Act
            var references = ReflectionTool.GetReferences<ChildClass1>(target).ToArray();

            // Assert
            Assert.AreEqual(3, references.Length, "Tool did not detect all references");
            Assert.AreEqual(0, references[0].Count());
            Assert.AreEqual(0, references[1].Count());
            Assert.AreEqual(0, references[2].Count());
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
                Children1 = new List<ChildClass1>(),
                Children2 = new List<ChildClass2>(),
                EmptyArray = new GranChildClass1[0],
                NullArray = null
            };

            // Act
            var references = ReflectionTool.GetReferences<BaseClass>(target, p => p.Name.StartsWith("Child")).ToArray();

            // Assert
            Assert.AreEqual(3, references.Length, "Tool did not detect all references");
        }
    }
}
