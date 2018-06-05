using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Marvin.Serialization
{
    /// <summary>
    /// Collection type for typed representations of config collections
    /// </summary>
    /// <typeparam name="T">Type of elements in the collection</typeparam>
    public class EntryCollection<T> : INotifyPropertyChanged, INotifyCollectionChanged, IEnumerable<T>, IEntryCollection
        where T : class, new()
    {
        /// <summary>
        /// Converter to transform between generic and the config model
        /// </summary>
        private static readonly EntryToModelConverter Converter = EntryToModelConverter.Create<T>();

        /// <summary>
        /// Root entry in the config model
        /// </summary>
        private readonly Entry _collectionRoot;

        /// <summary>
        /// Internally we wrap an observable collection
        /// </summary>
        private readonly IList<ModelAndEntry> _internalCollection = new List<ModelAndEntry>();

        /// <summary>
        /// Create a new instance of the collection
        /// </summary>
        /// <param name="collectionRoot"></param>
        public EntryCollection(Entry collectionRoot)
        {
            _collectionRoot = collectionRoot;
            foreach (var subEntry in collectionRoot.SubEntries)
            {
                _internalCollection.Add(Convert(subEntry));
            }
            LoadPrototype();
        }

        /// <summary>
        /// Currently selected type
        /// </summary>
        public string CurrentType
        {
            get { return _collectionRoot.Value.Current; }
            set
            {
                _collectionRoot.Value.Current = value;
                LoadPrototype();
            }
        }

        /// <summary>
        /// Possible entries that can be used to create instances
        /// </summary>
        public string[] PossibleTypes => _collectionRoot.Value.Possible;

        private ModelAndEntry _prototype;

        /// <summary>
        /// Bindable prototype instance
        /// </summary>
        public T Prototype => _prototype?.Instance;

        /// <summary>
        /// Adds the prototype instance to the collection
        /// </summary>
        public void AddPrototype()
        {
            Add(_prototype);
            LoadPrototype();
        }

        /// <summary>
        /// Create a prototype instance
        /// </summary>
        private void LoadPrototype()
        {
            var entry = _collectionRoot.Prototypes.First(p => p.Value.Current == (CurrentType ?? PossibleTypes[0]));
            _prototype = Convert(entry.Instantiate());
            RaisePropertyChanged("Prototype");
        }

        /// <summary>
        /// Add a new entry to the collection
        /// </summary>
        public void Add(string type)
        {
            var newEntry = _collectionRoot.GetPrototype(type).Instantiate();
            var converted = Convert(newEntry);
            Add(converted);
        }

        private void Add(ModelAndEntry newItem)
        {
            _internalCollection.Add(newItem);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem.Instance));
        }

        /// <summary>
        /// Remove item from collection
        /// </summary>
        public void Remove(T item)
        {
            for (var index = 0; index < _internalCollection.Count; index++)
            {
                if (_internalCollection[index].Instance != item)
                    continue;

                _internalCollection.RemoveAt(index);
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
                break;
            }
        }

        /// <summary>
        /// Export the config entries for the current
        /// </summary>
        /// <returns></returns>
        List<Entry> IEntryCollection.ConfigEntries()
        {
            return _internalCollection.Select(WriteToEntry).ToList();
        }

        /// <summary>
        /// Write instance values to model
        /// </summary>
        private static Entry WriteToEntry(ModelAndEntry item)
        {
            Converter.ToConfig(item.Instance, item.Model.SubEntries);
            return item.Model;
        }

        /// <summary>
        /// Event raised when the collection changes
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Redirect the change event switching the sender
        /// </summary>
        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
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
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
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
        public IEnumerator<T> GetEnumerator()
        {
            return _internalCollection.Select(e => e.Instance).GetEnumerator();
        }

        /// <summary>
        /// Convert entry to object and save both as a pair
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private static ModelAndEntry Convert(Entry model)
        {
            var instance = new T();
            Converter.FromConfig(model.SubEntries, instance);
            return new ModelAndEntry
            {
                Model = model,
                Instance = instance,
            };
        }

        private class ModelAndEntry
        {
            public T Instance { get; set; }

            public Entry Model { get; set; }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}[{1}]", typeof(T).Name, _internalCollection.Count);
        }


    }
}