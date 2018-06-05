using System;
using System.Linq;
using Marvin.Tools;
using NUnit.Framework;

namespace Marvin.Tests.Extensions
{
    [TestFixture]
    public class AppDomainExtensionTests
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
    }
}