using System;
using System.ComponentModel;
using System.Reflection;
using Marvin.Container;
using Marvin.Modules;
using Marvin.Modules.ModulePlugins;
using Marvin.Runtime.Configuration;
using Marvin.Runtime.Container;
using Marvin.Runtime.Kernel;
using Marvin.Runtime.ModuleManagement;
using NUnit.Framework;

namespace Marvin.Runtime.Tests
{
    /// <summary>
    /// Unittest to test the settings of attributes
    /// </summary>
    [TestFixture]
    public class AttributesTest
    {
        /// <summary>
        /// Checks the attribute settings.
        /// </summary>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <param name="baseType">The base type.</param>
        /// <param name="targets">The targets.</param>
        /// <param name="allowMultiple">if set to <c>true</c> [allow multiple].</param>
        /// <param name="inherited">if set to <c>true</c> [inherited].</param>
        [TestCase(typeof(ExpectedConfigAttribute), typeof(Attribute), AttributeTargets.Class, false, true, TestName = "ExpectedConfigAttribute")]
        [TestCase(typeof(ModuleStrategyAttribute), typeof(Attribute), AttributeTargets.All, false, true, TestName = "ModuleStrategyAttribute")]
        [TestCase(typeof(PossibleConfigValuesAttribute), typeof(Attribute), AttributeTargets.Property, false, true, TestName = "PossibleConfigValuesAttribute")]
        [TestCase(typeof(SharedConfigAttribute), typeof(Attribute), AttributeTargets.Property, false, true, TestName = "SharedConfigAttribute")]
        [TestCase(typeof(GlobalComponentAttribute), typeof(RegistrationAttribute), AttributeTargets.Class, false, true, TestName = "GlobalComponentAttribute")]
        [TestCase(typeof(KernelComponentAttribute), typeof(GlobalComponentAttribute), AttributeTargets.Class, false, true, TestName = "KernelComponentAttribute")]
        [TestCase(typeof(ServerModuleAttribute), typeof(GlobalComponentAttribute), AttributeTargets.Class, false, true, TestName = "ServerModuleAttribute")]
        [TestCase(typeof(DependencyRegistrationAttribute), typeof(Attribute), AttributeTargets.Class, false, true, TestName = "DependencyRegistrationAttribute")]
        [TestCase(typeof(RequiredModuleApiAttribute), typeof(Attribute), AttributeTargets.Property, false, true, TestName = "RequiredModuleApiAttribute")]
        [TestCase(typeof(KernelBundleAttribute), typeof(BundleAttribute), AttributeTargets.Assembly, false, true, TestName = "KernelBundleAttribute")]
        [TestCase(typeof(BundleAttribute), typeof(Attribute), AttributeTargets.Assembly, false, true, TestName = "KernelBundleAttribute")]
        [TestCase(typeof(CpuCountAttribute), typeof(DefaultValueAttribute), AttributeTargets.Property, false, true, TestName = "CpuCountAttribute")]
        [TestCase(typeof(CurrentHostNameAttribute), typeof(DefaultValueAttribute), AttributeTargets.Property, false, true, TestName = "CurrentHostNameAttribute")]
        [TestCase(typeof(IntegerStepsAttribute), typeof(PossibleConfigValuesAttribute), AttributeTargets.Property, false, true, TestName = "IntegerStepsAttribute")]
        [TestCase(typeof(PluginConfigsAttribute), typeof(PossibleConfigValuesAttribute), AttributeTargets.Property, false, true, TestName = "PluginConfigsAttribute")]
        [TestCase(typeof(PluginNameSelectorAttribute), typeof(PossibleConfigValuesAttribute), AttributeTargets.Property, false, true, TestName = "PluginNameSelectorAttribute")]
        [TestCase(typeof(RelativeDirectoriesAttribute), typeof(PossibleConfigValuesAttribute), AttributeTargets.Property, false, true, TestName = "RelativeDirectoriesAttribute")]
        [TestCase(typeof(PossibleValuesAttribute), typeof(PossibleConfigValuesAttribute), AttributeTargets.Property, false, true, TestName = "PossibleValuesAttribute")]
        public void CheckAttributeSettings(Type attributeType, Type baseType, AttributeTargets targets, bool allowMultiple, bool inherited)
        {
            // Check if it is a attribute
            Assert.True(typeof(Attribute).IsAssignableFrom(attributeType), "The attribute is not assignable to Attribute!");
            // Check attribute inheritance
            Assert.True(attributeType.BaseType == baseType, $"The attribute do not inherit from '{baseType}'!");

            var usageAttribute = attributeType.GetCustomAttribute<AttributeUsageAttribute>();

            Assert.NotNull(usageAttribute, "The attribute did not have a AttributeUsageAttribute.");
            Assert.AreEqual(targets,usageAttribute.ValidOn, "The attibute usage is unexpected!");
            Assert.AreEqual(allowMultiple, usageAttribute.AllowMultiple, "The setting for multiple instances is not correct!");
            Assert.AreEqual(inherited, usageAttribute.Inherited, "The inheritance setting is not correct!");
        }
    }
}