// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
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
                        Assert.That(entry.Description, Is.EqualTo(descriptionattribute.Description), "The description doesn't match to the attributes value!");
                    }

                    attribute = attr.FirstOrDefault(o => o is DisplayNameAttribute);
                    if (attribute == null)
                    {
                        foundPropertyWithDisplayNameAttribute = true;
                        Assert.That(propertyInfo.Name, Is.EqualTo(entry.DisplayName), "The display name is not equal to the property name!");
                    }
                    else
                    {
                        var defaultValueAttribute = (DisplayNameAttribute)attribute;
                        foundPropertyWithDisplayNameAttribute = true;
                        Assert.That(entry.DisplayName, Is.EqualTo(defaultValueAttribute.DisplayName), "The display name doen't match to the attributes value!");
                    }

                    // check for default value attributes
                    attribute = attr.FirstOrDefault(o => o is DefaultValueAttribute);
                    if (attribute == null)
                    {
                        // collections are using the default to safe the type.
                        if (typeof(IList).IsAssignableFrom(propertyInfo.PropertyType))
                        {
                            Assert.That(entry.Value.Default, Is.Not.Null, "The default value should contain the type of list!");
                            var args = propertyInfo.PropertyType.GenericTypeArguments;
                            Assert.That(args.Length, Is.EqualTo(1), "There are more generic argumets then expected!");
                            Assert.That(args[0].Name, Is.EqualTo(entry.Value.Default), "List should save the generic type name to the default field.");
                        }
                        else if (propertyInfo.PropertyType.IsValueType)
                        {
                            foundPropertyWithoutDefaultAttribute = true;
                            Assert.That(entry.Value.Default, Is.Not.Null, "Value types must not be null");
                        }
                    }
                    else
                    {
                        var defaultAttribute = (DefaultValueAttribute)attribute;
                        foundPropertyWithDefaultAttribute = true;
                        Assert.That(entry.Value.Default, Is.EqualTo(defaultAttribute.Value.ToString()), "The default value is not matching to the attibutes value!");
                    }

                    // check for possiblevalues attributes
                    if (propertyInfo.PropertyType.IsEnum)
                    {
                        foundPropertyWithoutValueAttribute = true;
                        Assert.That(entry.Value.Possible, Is.Not.Null, "Enums always have their default values!");
                    }
                    // collections are using the default to safe the type.
                    else if (typeof(IList).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        Assert.That(entry.Value.Possible, Is.Not.Null, "The possible value should contain the type of list!");

                        var args = propertyInfo.PropertyType.GenericTypeArguments;
                        Assert.That(args.Length, Is.EqualTo(1), "There are more  generic arguments then expected.");
                        Assert.That(entry.Value.Possible.Length, Is.EqualTo(1), "There are more possible vaule entries then expected!");
                        Assert.That(args[0].Name, Is.EqualTo(entry.Value.Possible[0]), "List should contain the generic type name in the list of possible values.");
                    }
                    else
                    {
                        attribute = attr.FirstOrDefault(o => o is PossibleValuesAttribute);
                        if (attribute == null)
                        {
                            foundPropertyWithoutValueAttribute = true;
                            Assert.That(entry.Value.Possible, Is.Null,
                                "There should be no limitation to possible values!");
                        }
                        else
                        {
                            var possibleValuesAttribute = (PossibleValuesAttribute)attribute;
                            foundPropertyWithValuesAttribute = true;
                            foreach (var value in possibleValuesAttribute.GetValues(null, null))
                            {
                                Assert.That(entry.Value.Possible, Does.Contain(value),
                                    "The value is not in the list of possible values!");
                            }
                        }
                    }

                    var propertyValue = propertyInfo.GetValue(config);
                    if (propertyValue == null)
                        Assert.That(entry.Value.Current, Is.EqualTo(entry.Value.Default), "The current value do not match.");
                    else if (typeof(IList).IsAssignableFrom(propertyInfo.PropertyType))
                        Assert.That(entry.SubEntries.Count, Is.EqualTo(((IList)propertyValue).Count));
                    else if (propertyInfo.PropertyType.IsClass && propertyInfo.PropertyType != typeof(string))
                        Assert.That(entry.Value.Current, Is.EqualTo(propertyInfo.PropertyType.Name));
                    else
                        Assert.That(propertyValue.ToString(), Is.EqualTo(entry.Value.Current), "The current value do not match.");
                    break;
                }
                Assert.That(found, "Property is missing: {0}", propertyInfo.Name);
            }

            // Check if i forgot some case in the test!
            Assert.That(foundPropertyWithDefaultAttribute, "Testscenario is incomplete or faulty! Missing property with default attribute!");
            Assert.That(foundPropertyWithoutDefaultAttribute, "Testscenario is incomplete or faulty! Missing property without default attribute!");
            Assert.That(foundPropertyWithDescriptionAttribute, "Testscenario is incomplete or faulty! Missing property with description attribute!");
            Assert.That(foundPropertyWithoutDescriptionAttribute, "Testscenario is incomplete or faulty! Missing property without description attribute!");
            Assert.That(foundPropertyWithValuesAttribute, "Testscenario is incomplete or faulty! Missing property with values attribute!");
            Assert.That(foundPropertyWithoutValueAttribute, "Testscenario is incomplete or faulty! Missing property without values attribute!");
            Assert.That(foundPropertyWithDisplayNameAttribute, "Testscenario is incomplete or faulty! Missing property with displayname attribute!");
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
            Assert.That(config.IntField.ToString(), Is.EqualTo(intFieldEntry.Value.Current), "Initially the the gerneric and the object must be the same.");
            // change the value
            intFieldEntry.Value.Current = "999";
            // check that it has changed.
            Assert.That(intFieldEntry.Value.Current, Is.Not.EqualTo(config.IntField.ToString()), "The generic must be changed!");
            // save changes
            EntryConvert.UpdateInstance(config, convertedObject);
            //provider.SetConfig(config);
            // check changes are safed to the config object.
            Assert.That(config.IntField.ToString(), Is.EqualTo(intFieldEntry.Value.Current), "After set, both must be the same.");
        }
    }
}
