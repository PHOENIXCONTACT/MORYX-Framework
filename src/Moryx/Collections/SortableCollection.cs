// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;

namespace Moryx.Collections
{
    /// <summary>
    /// Represents an object which can be sorted whithin a <see cref="ISortableCollection{T}"/>
    /// </summary>
    public interface ISortableObject
    {
        /// <summary>
        /// Sort order of this object whithin the <see cref="ISortableCollection{T}"/>
        /// </summary>
        int SortOrder { get; set; }
    }

    /// <summary>
    /// Special collection to handle positions of elements within this collection
    /// </summary>
    /// <typeparam name="T">The type of the elements whithin this collection</typeparam>
    public interface ISortableCollection<T> : ICollection<T> where T : ISortableObject
    {
        /// <summary>
        /// Gets the element with the array operator at the given index
        /// </summary>
        T this[int index] { get; }

        /// <summary>
        /// Will flush the internal modification cache and returns a collection of modiefied items
        /// </summary>
        ICollection<T> FlushModifications();

        /// <summary>
        /// Returns the index of the given item
        /// </summary>
        int IndexOf(T item);

        /// <summary>
        /// Moves the item one entry upwards
        /// </summary>
        /// <param name="item">The item to move</param>
        /// <returns>A collection of items which were changed while moving up</returns>
        void MoveUp(T item);

        /// <summary>
        /// Moves the item one entry down
        /// </summary>
        /// <param name="item">The item to move.</param>
        /// <returns>A collection of items which were changed while moving down.</returns>
        void MoveDown(T item);

        /// <summary>
        /// Inserts an item before another item.
        /// </summary>
        /// <param name="item">The reference item to insert an item before it.</param>
        /// <param name="itemToInsert">The item to insert.</param>
        /// <returns>A collection of items which were changed while inserting.</returns>
        void InsertBefore(T item, T itemToInsert);

        /// <summary>
        /// Inserts an item after another item.
        /// </summary>
        /// <param name="item">The reference item to insert an item after it.</param>
        /// <param name="itemToInsert">The item to insert.</param>
        /// <returns>A collection of items which were changed while inserting.</returns>
        void InsertAfter(T item, T itemToInsert);
    }

    /// <summary>
    /// Special collection to handle positions of elements within this collection
    /// </summary>
    /// <typeparam name="T">The type of the elements whithin this collection</typeparam>
    public class SortableCollection<T> : ISortableCollection<T> where T : class, ISortableObject
    {
        private readonly List<T> _items = [];
        private readonly List<T> _changeCache = [];

        /// 
        public T this[int index]
        {
            get { return _items[index]; }
        }

        /// 
        public void Add(T item)
        {
            _items.Add(item);

            //Add to the end of the internal list and set sort order
            item.SortOrder = _items.Count > 0 ? _items.Max(i => i.SortOrder) + 1 : 1;
        }

        /// 
        public bool Remove(T item)
        {
            var removeResult = _items.Remove(item);
            ReorderItems();

            return removeResult;
        }

        /// 
        public ICollection<T> FlushModifications()
        {
            var modifications = _changeCache.ToList();
            _changeCache.Clear();

            return modifications;

        }

        /// 
        public void MoveUp(T item)
        {
            var itemIndex = GetItemIndex(item);

            //Check if the given item is the first
            if (itemIndex == 0)
                return;

            //Move item up and return changed items
            Move(item, 1);
        }

        /// 
        public void MoveDown(T item)
        {
            var itemIndex = GetItemIndex(item);

            //Check if the given item is the last
            if (itemIndex == _items.Count - 1)
                return;

            //Move item down and return changed items
            Move(item, -1);
        }

        private void Move(T item, int offset)
        {
            var targetIndex = _items.IndexOf(item) - offset;
            var targetItem = _items[targetIndex];

            _items.Remove(item);
            _items.Insert(targetIndex, item);

            item.SortOrder = targetItem.SortOrder;
            targetItem.SortOrder = item.SortOrder + offset;

            AddItemsToModificationCache([item, targetItem]);
        }

        /// 
        public void InsertBefore(T item, T itemToInsert)
        {
            var itemIndex = GetItemIndex(item);

            //Insert at index of target item
            _items.Insert(itemIndex, itemToInsert);

            //Reorder complete list and add changes to modification cache
            ReorderItems();
        }

        /// 
        public void InsertAfter(T item, T itemToInsert)
        {
            var itemIndex = GetItemIndex(item);

            //Insert at index of the item after the target item
            _items.Insert(itemIndex + 1, itemToInsert);

            //Reorder complete list and add changes to modification cache
            ReorderItems();
        }

        /// 
        public int Count
        {
            get { return _items.Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        /// 
        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// 
        public void Clear()
        {
            _items.Clear();
        }

        /// 
        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        /// 
        public void CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        /// 
        public int IndexOf(T item)
        {
            return _items.IndexOf(item);
        }

        /// <summary>
        /// Returns the index of an item, if and only if the item is existent in the underlying list
        /// </summary>
        private int GetItemIndex(T item)
        {
            var itemIndex = _items.IndexOf(item);
            if (itemIndex < 0)
                throw new ArgumentException("Item was not found in underlying collection.", "item");
            return itemIndex;
        }

        /// <summary>
        /// Will check the cache for the given items and will add them if they are not in the list
        /// </summary>
        private void AddItemsToModificationCache(IEnumerable<T> items)
        {
            foreach (var item in items.Where(item => !_changeCache.Contains(item)))
            {
                _changeCache.Add(item);
            }
        }

        /// <summary>
        /// Reorders the underlying list and returns all changed items
        /// </summary>
        private void ReorderItems()
        {
            var changedItems = new List<T>();
            var index = 0;
            foreach (var item in _items)
            {
                var sortIndex = index + 1;
                if (item.SortOrder != sortIndex)
                {
                    item.SortOrder = sortIndex;
                    changedItems.Add(item);
                }

                index++;
            }

            //Resort the underlying list
            _items.Sort((a, b) => a.SortOrder.CompareTo(b.SortOrder));

            AddItemsToModificationCache(changedItems);
        }
    }
}
