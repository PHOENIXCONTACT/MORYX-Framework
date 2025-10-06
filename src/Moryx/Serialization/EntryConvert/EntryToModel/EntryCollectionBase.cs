// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Moryx.Serialization
{
    /// <summary>
    /// Base class for EntryCollections
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EntryCollectionBase<T> : INotifyPropertyChanged, INotifyCollectionChanged, IEnumerable<T>, IEntryCollection
        where T : class, new()
    {
        /// <summary>
        /// Converter to transform between generic and the config model
        /// </summary>
        protected static readonly EntryToModelConverter Converter = EntryToModelConverter.Create<T>();

        /// <summary>
        /// Root entry in the config model
        /// </summary>
        protected readonly Entry CollectionRoot;

        private ModelAndEntry _prototype;

        /// <summary>
        /// Create a new instance of the collection
        /// </summary>
        /// <param name="collectionRoot">Collection used as root</param>
        protected EntryCollectionBase(Entry collectionRoot)
        {
            CollectionRoot = collectionRoot;
        }

        /// <summary>
        /// Currently selected type
        /// </summary>
        public string CurrentType
        {
            get { return CollectionRoot.Value.Current; }
            set
            {
                CollectionRoot.Value.Current = value;
                LoadPrototype();
            }
        }

        /// <summary>
        /// Possible entries that can be used to create instances
        /// </summary>
        public string[] PossibleTypes => CollectionRoot.Value.Possible;

        /// <summary>
        /// Bind able prototype instance
        /// </summary>
        public T Prototype => _prototype?.Instance;

        /// <summary>
        /// Internal prototype
        /// </summary>
        protected ModelAndEntry InternalPrototype => _prototype;

        /// <summary>
        /// Create a prototype instance
        /// </summary>
        protected void LoadPrototype()
        {
            var entry = CollectionRoot.Prototypes.First(p => p.Value.Current == (CurrentType ?? PossibleTypes[0]));
            _prototype = Convert(entry.Instantiate());
            RaisePropertyChanged(nameof(Prototype));
        }

        /// <summary>
        /// Export the config entries for the current
        /// </summary>
        /// <returns></returns>
        protected abstract List<Entry> GetConfigEntries();

        List<Entry> IEntryCollection.ConfigEntries()
        {
            return GetConfigEntries();
        }

        /// <summary>
        /// Event raised when the collection changes
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Redirect the change event switching the sender
        /// </summary>
        protected void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var handler = CollectionChanged;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Event raised when the properties changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Will raise the <see cref="PropertyChanged"/> event
        /// </summary>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public abstract IEnumerator<T> GetEnumerator();

        /// <summary>
        /// Convert entry to object and save both as a pair
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected static ModelAndEntry Convert(Entry model)
        {
            var instance = new T();
            Converter.FromModel(model, instance);
            return new ModelAndEntry
            {
                Model = model,
                Instance = instance,
            };
        }

        /// <summary>
        /// Helper class that holds an instance and the corresponding model for internal use
        /// </summary>
        protected class ModelAndEntry
        {
            /// <summary>
            /// The instance
            /// </summary>
            public T Instance { get; set; }

            /// <summary>
            /// Entries of the given <see cref="Instance"/>
            /// </summary>
            public Entry Model { get; set; }
        }
    }
}
