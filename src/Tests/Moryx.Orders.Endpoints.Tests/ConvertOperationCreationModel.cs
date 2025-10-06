// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Orders.Endpoints.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Moryx.Orders.Endpoints.Tests
{
    [TestFixture]
    public class ConvertOperationCreationModel
    {
        private OperationCreationContext _context1;
        private OperationCreationContext _context2;

        [SetUp]
        public void SetUp()
        {
            _context1 = new OperationCreationContext()
            {
                Order = new OrderCreationContext() { Number = "12345" },
                TotalAmount = 5,
                Name = "abc",
                Number = "42",
                ProductIdentifier = "1234321",
                ProductRevision = 12,
                ProductName = "sdf",
                RecipePreselection = 2,
                OverDeliveryAmount = 3,
                UnderDeliveryAmount = 4,
                PlannedStart = DateTime.Now,
                PlannedEnd = DateTime.Now,
                TargetCycleTime = 3,
                Unit = "s",
                TargetStock = "ab",
                Parts = new List<PartCreationContext> { new() },
                MaterialParameters = new List<IMaterialParameter> { new DummyMaterialParameter() { Color = "Green" } }
            };

            _context2 = new OperationCreationContext()
            {
                Order = new OrderCreationContext() { Number = "12345" },
                TotalAmount = 5,
                Name = "abc",
                Number = "42",
                ProductIdentifier = "1234321",
                ProductRevision = 12,
                ProductName = null,
                RecipePreselection = 2,
                OverDeliveryAmount = 3,
                UnderDeliveryAmount = 4,
                PlannedStart = DateTime.Now,
                PlannedEnd = DateTime.Now,
                TargetCycleTime = 3,
                Unit = "s",
                TargetStock = "ab",
                Parts = new List<PartCreationContext> { new() },
                MaterialParameters = null
            };
        }

        [Test(Description = "Convert OperationCreationContext containing null values to model and then convert it back")]
        public void ConvertModelWithNullValues()
        {
            var model = new OperationCreationContextModel(_context2);
            var convertedContext = model.ConvertToContext();
            Assert.That(convertedContext, Is.Not.Null);
            Assert.That(convertedContext.Order.Number, Is.EqualTo(_context2.Order.Number));
            Assert.That(convertedContext.Name, Is.EqualTo(_context2.Name));
            Assert.That(convertedContext.Number, Is.EqualTo(_context2.Number));
            Assert.That(convertedContext.ProductIdentifier, Is.EqualTo(_context2.ProductIdentifier));
            Assert.That(convertedContext.ProductRevision, Is.EqualTo(_context2.ProductRevision));
            Assert.That(convertedContext.ProductName, Is.EqualTo(_context2.ProductName));
            Assert.That(convertedContext.RecipePreselection, Is.EqualTo(_context2.RecipePreselection));
            Assert.That(convertedContext.OverDeliveryAmount, Is.EqualTo(_context2.OverDeliveryAmount));
            Assert.That(convertedContext.UnderDeliveryAmount, Is.EqualTo(_context2.UnderDeliveryAmount));
            Assert.That(convertedContext.PlannedStart, Is.EqualTo(_context2.PlannedStart));
            Assert.That(convertedContext.PlannedEnd, Is.EqualTo(_context2.PlannedEnd));
            Assert.That(convertedContext.TargetCycleTime, Is.EqualTo(_context2.TargetCycleTime));
            Assert.That(convertedContext.Unit, Is.EqualTo(_context2.Unit));
            Assert.That(convertedContext.TargetStock, Is.EqualTo(_context2.TargetStock));
            Assert.That(convertedContext.Parts.Count, Is.EqualTo(_context2.Parts.Count));
            Assert.That(convertedContext.MaterialParameters, Is.Empty);
        }

        [Test(Description = "Convert OperationCreationContext to model and then convert it back")]
        public void ConvertModel()
        {
            var model = new OperationCreationContextModel(_context1);
            var convertedContext = model.ConvertToContext();
            Assert.That(convertedContext, Is.Not.Null);
            Assert.That(convertedContext.Order.Number, Is.EqualTo(_context1.Order.Number));
            Assert.That(convertedContext.Name, Is.EqualTo(_context1.Name));
            Assert.That(convertedContext.Number, Is.EqualTo(_context1.Number));
            Assert.That(convertedContext.ProductIdentifier, Is.EqualTo(_context1.ProductIdentifier));
            Assert.That(convertedContext.ProductRevision, Is.EqualTo(_context1.ProductRevision));
            Assert.That(convertedContext.ProductName, Is.EqualTo(_context1.ProductName));
            Assert.That(convertedContext.RecipePreselection, Is.EqualTo(_context1.RecipePreselection));
            Assert.That(convertedContext.OverDeliveryAmount, Is.EqualTo(_context1.OverDeliveryAmount));
            Assert.That(convertedContext.UnderDeliveryAmount, Is.EqualTo(_context1.UnderDeliveryAmount));
            Assert.That(convertedContext.PlannedStart, Is.EqualTo(_context1.PlannedStart));
            Assert.That(convertedContext.PlannedEnd, Is.EqualTo(_context1.PlannedEnd));
            Assert.That(convertedContext.TargetCycleTime, Is.EqualTo(_context1.TargetCycleTime));
            Assert.That(convertedContext.Unit, Is.EqualTo(_context1.Unit));
            Assert.That(convertedContext.TargetStock, Is.EqualTo(_context1.TargetStock));
            Assert.That(convertedContext.Parts.Count, Is.EqualTo(_context1.Parts.Count));
            Assert.That(convertedContext.MaterialParameters.Count, Is.EqualTo(_context1.MaterialParameters.Count));
            var originalMaterialParameter = _context1.MaterialParameters.ToArray()[0] as DummyMaterialParameter;
            var convertedMaterialParameter = convertedContext.MaterialParameters.ToArray()[0] as DummyMaterialParameter;
            Assert.That(convertedMaterialParameter, Is.Not.Null);
            Assert.That(convertedMaterialParameter.Color, Is.EqualTo(originalMaterialParameter.Color));
        }

    }

    public class DummyMaterialParameter : IMaterialParameter
    {
        public string Color { get; set; }
    }
}

