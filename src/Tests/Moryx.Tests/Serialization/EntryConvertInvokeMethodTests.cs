// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Moryx.Serialization;
using NUnit.Framework;

namespace Moryx.Tests
{
    /// <summary>
    /// Unit tests for the static <see cref="EntryToModelConverter"/> class
    /// </summary>
    [TestFixture]
    public class EntryConvertInvokeMethodTests
    {
        private readonly EntrySerialize_Methods _sut;
        private readonly EntrySerializeSerialization _serialization;

        public EntryConvertInvokeMethodTests() {
            _sut = new EntrySerialize_Methods();
            //_serialization = new EntrySerializeSerialization();
        }

        [Test]
        public void PublicMethodsWillBeInvoked()
        {
            // Act
            var entry = EntryConvert.InvokeMethod(_sut, NoParamsMethodEntry("InvocablePublic"));
            
            // Assert
            Assert.Null(entry);
        }

        [Test]
        public void InternalMethodsWillBeInvoked()
        {
            // Act
            var entry = EntryConvert.InvokeMethod(_sut, NoParamsMethodEntry("InvocableInternal"));

            // Assert
            Assert.Null(entry);
        }

        [Test]
        public void ProtectedMethodsWillNotBeInvoked()
        {
            // Act / Assert
            Assert.Throws<System.MissingMethodException>(
                () => EntryConvert.InvokeMethod(_sut, NoParamsMethodEntry("NonInvocableProtected")));
        }

        [Test]
        public void PrivateMethodsWillNotBeInvoked()
        {
            // Act / Assert
            Assert.Throws<System.MissingMethodException>(
                () => EntryConvert.InvokeMethod(_sut, NoParamsMethodEntry("NonInvocablePrivate")));
        }

        private MethodEntry NoParamsMethodEntry(string name)
            => new()
            { Name = name, Parameters = new(){ } };
    }
}
