// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using System.ComponentModel;
using System.Linq;
using Moryx.Configuration;
using Moryx.Serialization;
using NUnit.Framework;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace Moryx.Tests.Configuration
{
    /// <summary>
    /// Test for the config to model and model to config transformer
    /// </summary>
    [TestFixture]
    public class ModelTransformerTest
    {
        /// <summary>
        /// Tests the convertion of the config to the generic object.
        /// </summary>
        [Test(Description = "Checks the conversation result from config to generic object.")]
        public void ConvertTest()
        {
            var config = new TransformerTestConfig();

            // tranform a config object
            var serialization = new PossibleValuesSerialization(null, null);//new TransformationProviderMock(config));
            var converted = EntryConvert.EncodeObject(config, serialization);

            bool foundPropertyWithDescriptionAttribute = false;
            bool foundPropertyWithoutDescriptionAttribute = false;
            bool foundPropertyWithDefaultAttribute = false;
            bool foundPropertyWithoutDefaultAttribute = false;
            bool foundPropertyWithValuesAttribute = false;
            bool foundPropertyWithoutValueAttribute = false;
            bool foundPropertyWithDisplayNameAttribute = false;

            // get the properties of the given object
            foreach (var propertyInfo in config.GetType().GetProperties())
            {
                bool found = false;
                foreach (var entry in converted.SubEntries)
                {
                    // find the property in the generic object
                    if (propertyInfo.Name != entry.Identifier) 
                        continue;

                    found = true;
                    var attr = propertyInfo.GetCustomAttributes(false);

                    // check for description attributes
                    var attribute = attr.FirstOrDefault(o => o is DescriptionAttribute);
                    if (attribute == null)
                    {
                        foundPropertyWithoutDescriptionAttribute = true;
                        Assert.That(string.IsNullOrEmpty(entry.Description), "The description is not null!");
                    }
                    else
                    {
                        var descriptionattribute = (DescriptionAttribute)attribute;
                        foundPropertyWithDescriptionAttribute = true;
                        Assert.AreEqual(descriptionattribute.Description, entry.Description, "The description doesn't match to the attributes value!");
                    }

                    attribute = attr.FirstOrDefault(o => o is DisplayNameAttribute);
                    if (attribute == null)
                    {
                        foundPropertyWithDisplayNameAttribute = true;
                        Assert.AreEqual(entry.DisplayName, propertyInfo.Name, "The display name is not equal to the property name!");
                    }
                    else
                    {
                        var defaultValueAttribute = (DisplayNameAttribute)attribute;
                        foundPropertyWithDisplayNameAttribute = true;
                        Assert.AreEqual(defaultValueAttribute.DisplayName, entry.DisplayName, "The display name doen't match to the attributes value!");
                    }

                    // check for default value attributes
                    attribute = attr.FirstOrDefault(o => o is DefaultValueAttribute);
                    if (attribute == null)
                    {
                        // collections are using the default to safe the type.
                        if (typeof(IList).IsAssignableFrom(propertyInfo.PropertyType))
                        {
                            Assert.IsNotNull(entry.Value.Default, "The default value should contain the type of list!");
                            var args = propertyInfo.PropertyType.GenericTypeArguments;
                            Assert.AreEqual(1, args.Length, "There are more generic argumets then expected!");
                            Assert.AreEqual(entry.Value.Default, args[0].Name, "List should save the generic type name to the default field.");
                        }
                        else if (propertyInfo.PropertyType.IsValueType)
                        {
                            foundPropertyWithoutDefaultAttribute = true;
                            Assert.NotNull(entry.Value.Default, "Value types must not be null");
                        }
                    }
                    else
                    {
                        var defaultAttribute = (DefaultValueAttribute)attribute;
                        foundPropertyWithDefaultAttribute = true;
                        Assert.AreEqual(defaultAttribute.Value.ToString(), entry.Value.Default, "The default value is not matching to the attibutes value!");
                    }

                    // check for possiblevalues attributes
                    if (propertyInfo.PropertyType.IsEnum)
                    {
                        foundPropertyWithoutValueAttribute = true;
                        Assert.IsNotNull(entry.Value.Possible, "Enums always have their default values!");
                    }
                        // collections are using the default to safe the type.
                    else if (typeof(IList).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        Assert.IsNotNull(entry.Value.Possible, "The possible value should contain the type of list!");

                        var args = propertyInfo.PropertyType.GenericTypeArguments;
                        Assert.AreEqual(1, args.Length, "There are more  generic arguments then expected.");
                        Assert.AreEqual(1, entry.Value.Possible.Length, "There are more possible vaule entries then expected!");
                        Assert.AreEqual(entry.Value.Possible[0], args[0].Name, "List should contain the generic type name in the list of possible values.");
                    }
                    else
                    {
                        attribute = attr.FirstOrDefault(o => o is PossibleValuesAttribute);
                        if (attribute == null)
                        {
                            foundPropertyWithoutValueAttribute = true;
                            Assert.IsNull(entry.Value.Possible,
                                "There should be no limitation to possible values!");
                        }
                        else
                        {
                            var possibleValuesAttribute = (PossibleValuesAttribute) attribute;
                            foundPropertyWithValuesAttribute = true;
                            foreach (var value in possibleValuesAttribute.GetValues(null))
                            {
                                Assert.Contains(value, entry.Value.Possible,
                                    "The value is not in the list of possible values!");
                            }
                        }
                    }

                    var propertyValue = propertyInfo.GetValue(config);
                    if (propertyValue == null)
                        Assert.AreEqual(entry.Value.Default,entry.Value.Current, "The current value do not match.");
                    else if (typeof(IList).IsAssignableFrom(propertyInfo.PropertyType))
                        Assert.AreEqual(((IList)propertyValue).Count, entry.SubEntries.Count);
                    else if (propertyInfo.PropertyType.IsClass && propertyInfo.PropertyType != typeof(string))
                        Assert.AreEqual(propertyInfo.PropertyType.Name, entry.Value.Current);
                    else
                        Assert.AreEqual(propertyValue.ToString(), entry.Value.Current, "The current value do not match.");
                    break;
                }
                Assert.IsTrue(found, "Property is missing: {0}", propertyInfo.Name);
            }

            // Check if i forgot some case in the test!
            Assert.IsTrue(foundPropertyWithDefaultAttribute, "Testscenario is incomplete or faulty! Missing property with default attribute!");
            Assert.IsTrue(foundPropertyWithoutDefaultAttribute, "Testscenario is incomplete or faulty! Missing property without default attribute!");
            Assert.IsTrue(foundPropertyWithDescriptionAttribute, "Testscenario is incomplete or faulty! Missing property with description attribute!");
            Assert.IsTrue(foundPropertyWithoutDescriptionAttribute, "Testscenario is incomplete or faulty! Missing property without description attribute!");
            Assert.IsTrue(foundPropertyWithValuesAttribute, "Testscenario is incomplete or faulty! Missing property with values attribute!");
            Assert.IsTrue(foundPropertyWithoutValueAttribute, "Testscenario is incomplete or faulty! Missing property without values attribute!");
            Assert.IsTrue(foundPropertyWithDisplayNameAttribute, "Testscenario is incomplete or faulty! Missing property with displayname attribute!");
        }

        /// <summary>
        /// Test the set configuration and the request configuration entry function.
        /// </summary>
        [Test(Description = "Checks if the config object is updated.")]
        public void SetConfigTest()
        {
            TransformerTestConfig config = new TransformerTestConfig();

            //var provider = new TransformationProviderMock(config);
            // tranform a config object
            var configToModel = new PossibleValuesSerialization(null, null);
            var convertedObject = EntryConvert.EncodeObject(config, configToModel);

            // find the int field to chang its value
            var intFieldEntry = convertedObject.SubEntries.Find(entry => entry.DisplayName == "IntField");
            // check the initial value
            Assert.AreEqual(intFieldEntry.Value.Current, config.IntField.ToString(), "Initially the the gerneric and the object must be the same.");
            // change the value
            intFieldEntry.Value.Current = "999";
            // check that it has changed.
            Assert.AreNotEqual(intFieldEntry.Value.Current, config.IntField.ToString(), "The generic must be changed!");
            // save changes
            EntryConvert.UpdateInstance(config, convertedObject);
            //provider.SetConfig(config);
            // check changes are safed to the config object.
            Assert.AreEqual(intFieldEntry.Value.Current, config.IntField.ToString(), "After set, both must be the same.");
        }
    }
}
