// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.Modules;

namespace Moryx.ProcessData.Adapter.ResourceManagement
{
    [Plugin(LifeCycle.Singleton)]
    internal class ResourceManagementAdapter : IPlugin
    {
        private const string MeasurementPrefix = "resources_";

        private readonly ICollection<IProcessDataPublisher> _resources = new List<IProcessDataPublisher>();

        #region Dependencies

        public IResourceManagement ResourceManagement { get; set; }

        public IProcessDataMonitor ProcessDataMonitor { get; set; }

        #endregion

        /// <inheritdoc />
        public void Start()
        {
            ResourceManagement.ResourceAdded += OnResourceAdded;
            ResourceManagement.ResourceRemoved += OnResourceRemoved;

            var processDataPublishers = ResourceManagement.GetResources<IResource>().OfType<IProcessDataPublisher>();
            foreach (var publisher in processDataPublishers)
                RegisterToResource(publisher);
        }

        /// <summary>
        /// Stops the adapter component
        /// </summary>
        public void Stop()
        {
            ResourceManagement.ResourceAdded -= OnResourceAdded;
            ResourceManagement.ResourceRemoved -= OnResourceRemoved;

            foreach (var resource in _resources)
                resource.ProcessDataOccurred -= OnProcessDataOccurred;

            _resources.Clear();
        }

        private void OnResourceAdded(object sender, IResource addedResource)
        {
            if (addedResource is IProcessDataPublisher publisher)
                RegisterToResource(publisher);
        }

        private void OnResourceRemoved(object sender, IResource addedResource)
        {
            if (addedResource is IProcessDataPublisher publisher)
                UnregisterFromResource(publisher);
        }

        private void RegisterToResource(IProcessDataPublisher publisher)
        {
            _resources.Add(publisher);
            publisher.ProcessDataOccurred += OnProcessDataOccurred;
        }

        private void UnregisterFromResource(IProcessDataPublisher publisher)
        {
            _resources.Remove(publisher);
            publisher.ProcessDataOccurred -= OnProcessDataOccurred;
        }

        private void OnProcessDataOccurred(object sender, Measurement measurement)
        {
            ProcessDataMonitor.Add(measurement);
        }
    }
}
