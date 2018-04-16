using System.Linq;
using Marvin.AbstractionLayer.Resources;
using Marvin.TestTools.UnitTest;
using NUnit.Framework;

namespace Marvin.Resources.Management.Tests
{
    [TestFixture]
    public class ResourceLinkerTests
    {
        private IResourceLinker _linker;

        [OneTimeSetUp]
        public void Setup()
        {
            _linker = new ResourceLinker
            {
                Logger = new DummyLogger() 
            };
        }

        [Test]
        public void SetReferenceCollection()
        {
            // Arrange 
            var resource = new ReferenceResource();

            // Act
            _linker.SetReferenceCollections(resource);

            // Assert
            Assert.NotNull(resource.References);
            Assert.NotNull(resource.ChildReferences);
        }

        [Test]
        public void DetectAutoSaveCollections()
        {
            // Arrange
            var resource = new ReferenceResource();

            // Act
            _linker.SetReferenceCollections(resource);
            var autosave = _linker.GetAutoSaveCollections(resource);
            Assert.AreEqual(1, autosave.Count);

            // Validate event raised on modification
            var overrideAutosave = autosave.First();
            ReferenceCollectionChangedEventArgs eventArgs = null;
            overrideAutosave.CollectionChanged += (sender, args) => eventArgs = args;
            resource.ChildReferences.Add(new DerivedResource {Id = 42});

            // Assert
            Assert.NotNull(eventArgs);
            Assert.AreEqual(resource, eventArgs.Parent);
            Assert.AreEqual(nameof(Resource.Children), eventArgs.CollectionProperty.Name);
        }
    }
}