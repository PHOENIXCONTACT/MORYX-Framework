// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using Moryx.Collections;
using NUnit.Framework;

namespace Moryx.Tests.Collections
{
    [TestFixture]
    public class SortableCollectionTests
    {
        private SortableCollection<SortObj> _collection;

        private enum Movement
        {
            Up,
            Down
        }

        [SetUp]
        public void SetUp()
        {
            _collection = new SortableCollection<SortObj>
            {
                new SortObj("A"),
                new SortObj("B"),
                new SortObj("C"),
                new SortObj("D"),
            };
        }

        [TearDown]
        public void TearDown()
        {

        }
        
        [TestCase(0, 0, 1, TestName = "Move A up")]
        [TestCase(1, 0, 1, TestName = "Move B up")]
        [TestCase(2, 1, 2, TestName = "Move C up")]
        [TestCase(3, 2, 3, TestName = "Move D up")]
        public void MoveUpTest(int defaultIndex, int targetIndex, int targetOrder)
        {
            Move(defaultIndex, targetIndex, targetOrder, Movement.Up);
        }

        [TestCase(0, 1, 2, TestName = "Move A down")]
        [TestCase(1, 2, 3, TestName = "Move B down")]
        [TestCase(2, 3, 4, TestName = "Move C down")]
        [TestCase(3, 3, 4, TestName = "Move D down")]
        public void MoveDownTest(int defaultIndex, int targetIndex, int targetOrder)
        {
            Move(defaultIndex, targetIndex, targetOrder, Movement.Down);
        }

        private void Move(int defaultIndex, int targetIndex, int targetOrder, Movement movement)
        {
            var originItem = _collection[defaultIndex];
            var targetItem = _collection[targetIndex];

            if (movement == Movement.Up)
            {
                _collection.MoveUp(originItem);
            }
            else
            {
                _collection.MoveDown(originItem);
            }

            var changedItems = _collection.FlushModifications();
            
            Assert.AreEqual(targetOrder, originItem.SortOrder);

            if (defaultIndex != targetIndex)
            {
                Assert.True(changedItems.Contains(originItem), "Changed items does not contain the origin item");
                Assert.True(changedItems.Contains(targetItem), "Changed items does not contain the target item");
            }

            if (defaultIndex == targetIndex)
            {
                Assert.True(!changedItems.Any());
            }

            var itemIndex = _collection.IndexOf(originItem);
            Assert.AreEqual(targetIndex, itemIndex, "Item is not at the right position!");
        }

        [TestCase(0, 1, 5, TestName = "Insert E before A")]
        [TestCase(1, 2, 4, TestName = "Insert E before B")]
        [TestCase(2, 3, 3, TestName = "Insert E before C")]
        [TestCase(3, 4, 2, TestName = "Insert E before D")]
        public void InsertBeforeTest(int beforeItemIndex, int originTargetIndex, int changedItemsCount)
        {
            var beforeItem = _collection[beforeItemIndex];
            var newItem = new SortObj("E");

            _collection.InsertBefore(beforeItem, newItem);
            var changedItems = _collection.FlushModifications();

            Assert.AreEqual(originTargetIndex, _collection.IndexOf(beforeItem));
            Assert.AreEqual(changedItemsCount, changedItems.Count);
            Assert.IsTrue(changedItems.Contains(newItem));
        }

        [Test]
        public void UnknownItemTest()
        {
            var knownItem = _collection[0];
            var unknownItem = new SortObj("Devil");

            Assert.Throws<ArgumentException>(() => _collection.InsertBefore(unknownItem, knownItem));
            Assert.Throws<ArgumentException>(() => _collection.InsertAfter(unknownItem, knownItem));
            Assert.Throws<ArgumentException>(() => _collection.MoveUp(unknownItem));
            Assert.Throws<ArgumentException>(() => _collection.MoveDown(unknownItem));
        }

        [Test]
        public void AddItemTest()
        {
            var newItem = new SortObj("New Item");
            var currentItems = _collection.Count;

            _collection.Add(newItem);

            Assert.AreEqual(currentItems + 1, newItem.SortOrder);
        }

        [TestCase(0, 3)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(3, 0)]
        public void RemoveTest(int removeItemIndex, int changedItemsCount)
        {
            var itemToRemove = _collection[removeItemIndex];

            _collection.Remove(itemToRemove);

            var changedItems = _collection.FlushModifications();

            Assert.AreEqual(changedItemsCount, changedItems.Count);
        }
    }
}
