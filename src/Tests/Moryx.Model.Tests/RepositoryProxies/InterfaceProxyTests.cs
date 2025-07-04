// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Model.Repositories;
using Moryx.Model.Repositories.Proxy;
using NUnit.Framework;

namespace Moryx.Model.Tests
{
    [TestFixture]
    public class InterfaceProxyTests
    {
        private RepositoryProxyBuilder _proxyBuilder;

        [SetUp]
        public void SetUp()
        {
            _proxyBuilder = new RepositoryProxyBuilder();
        }

        [Test]
        public void EmptyRepositoryInterface()
        {
            // Act
            Type proxyType = null;
            Assert.DoesNotThrow(delegate
            {
                proxyType = _proxyBuilder.Build(typeof(IEmptyRepository));
            });

            // Assert
            var baseType = proxyType.BaseType;
            Assert.That(baseType, Is.Not.Null);

            var genericBaseType = baseType.GetGenericTypeDefinition();
            Assert.That(genericBaseType, Is.EqualTo(typeof(Repository<>)));
        }

        [Test]
        public void InterfaceWithoutRepositoryThrows()
        {
            //Act - Assert
            Assert.Throws<InvalidOperationException>(delegate
            {
                _proxyBuilder.Build(typeof(IWithoutIRepositoryRepository));
            });
        }

        [Test]
        public void NonRepositoryInterfaceThrows()
        {
            //Act - Assert
            Assert.Throws<InvalidOperationException>(delegate
            {
                _proxyBuilder.Build(typeof(Type));
            });
        }

        [Test]
        public void CreateWithStringParameter()
        {
            //Act - Assert
            Assert.DoesNotThrow(delegate
            {
                _proxyBuilder.Build(typeof(ICreateStringParamRepository));
            });
        }

        [Test]
        public void CreateWithValueParameter()
        {
            //Act - Assert
            Assert.DoesNotThrow(delegate
            {
                _proxyBuilder.Build(typeof(ICreateValueParamRepository));
            });
        }

        [Test]
        public void CreateWithUnknownPropertyThrows()
        {
            //Act - Assert
            Assert.Throws<InvalidOperationException>(delegate
            {
                _proxyBuilder.Build(typeof(ICreateUnknownPropertyRepository));
            });
        }

        [Test]
        public void CreateWithWrongReturnTypeThrows()
        {
            //Act - Assert
            Assert.Throws<InvalidOperationException>(delegate
            {
                _proxyBuilder.Build(typeof(ICreateWrongReturnTypeRepository));
            });
        }

        [Test]
        public void CreateWithAllParameters()
        {
            //Act
            var proxyType = _proxyBuilder.Build(typeof(ICreateAllParamsRepository));

            // Assert
            var baseType = proxyType.BaseType;
            Assert.That(baseType, Is.Not.Null);

            var genericBaseType = baseType.GetGenericTypeDefinition();
            Assert.That(genericBaseType, Is.EqualTo(typeof(Repository<>)));
        }

        [Test]
        public void CreateModificationTracked()
        {
            // Act
            var proxyType = _proxyBuilder.Build(typeof(IModificationTrackedRepository));

            // Assert
            var baseType = proxyType.BaseType;
            Assert.That(baseType, Is.Not.Null);

            var genericBaseType = baseType.GetGenericTypeDefinition();
            Assert.That(genericBaseType, Is.EqualTo(typeof(ModificationTrackedRepository<>)));
        }

        [Test]
        public void CreateWithWrongParameterExcepionIsThrown()
        {
            //Act - Assert
            Assert.Throws<InvalidOperationException>(delegate
            {
                _proxyBuilder.Build(typeof(IWrongParamTypeRepository));
            });

        }
    }
}
