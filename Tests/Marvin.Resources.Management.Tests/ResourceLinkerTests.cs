using System.Collections.Generic;
using System.Linq;
using Marvin.AbstractionLayer.Resources;
using Marvin.Model;
using Marvin.Resources.Model;
using Marvin.TestTools.UnitTest;
using Moq;
using NUnit.Framework;

namespace Marvin.Resources.Management.Tests
{
    [TestFixture]
    public class ResourceLinkerTests
    {
        private IResourceLinker _linker;

        [TestFixtureSetUp]
        public void Setup()
        {
            _linker = new ResourceLinker
            {
                Logger = new DummyLogger()
            };
        }

        [Test(Description = "Set all reference collections on the test resource including overrides")]
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

        [Test(Description = "Detect all collections flagged with 'AutoSave' on the test resource")]
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
            resource.ChildReferences.Add(new DerivedResource { Id = 42 });

            // Assert
            Assert.NotNull(eventArgs);
            Assert.AreEqual(resource, eventArgs.Parent);
            Assert.AreEqual(nameof(Resource.Children), eventArgs.CollectionProperty.Name);
        }

        [Test(Description = "Fill all references of the test resource")]
        public void LinkResource()
        {
            // Arrange
            var instance = new ReferenceResource { Id = 1 };
            var references = new Dictionary<long, ResourceWrapper>
            {
                [2] = new ResourceWrapper(new SimpleResource { Id = 2, Name = "Ref1" }),
                [3] = new ResourceWrapper(new SimpleResource { Id = 3, Name = "Pos1" }),
                [4] = new ResourceWrapper(new DerivedResource { Id = 4, Name = "Ref2" }),
                [5] = new ResourceWrapper(new DerivedResource { Id = 5, Name = "ChildOnly" }),
                [6] = new ResourceWrapper(new DerivedResource { Id = 6, Name = "BackRef" }),
            };
            var relations = new List<ResourceRelationAccessor>
            {
                // All Parent-Child relations
                RelationAccessor(2), RelationAccessor(3), RelationAccessor(4), RelationAccessor(5),
                RelationAccessor(6, ResourceRelationType.ParentChild, ResourceReferenceRole.Source),
                // All PossibleExchangeablePart relations
                RelationAccessor(2, ResourceRelationType.PossibleExchangablePart), RelationAccessor(3, ResourceRelationType.PossibleExchangablePart),
                RelationAccessor(4, ResourceRelationType.PossibleExchangablePart),
                // The 2 CurrentExchangeablePart
                RelationAccessor(2, ResourceRelationType.CurrentExchangablePart, ResourceReferenceRole.Target, nameof(ReferenceResource.Reference)),
                RelationAccessor(4, ResourceRelationType.CurrentExchangablePart),
            };

            // Act
            _linker.SetReferenceCollections(instance);
            _linker.LinkReferences(instance, relations, references);

            // Assert
            Assert.NotNull(instance.Parent, "Parent reference not set");
            Assert.NotNull(instance.Reference, "Named reference not set");
            Assert.NotNull(instance.Reference2, "Type inferred reference not set");
            Assert.AreEqual(4, instance.Children.Count, "Children not set");
            Assert.AreEqual(4, instance.ChildReferences.Count, "Children override not set");
            Assert.AreEqual(3, instance.References.Count, "Possible parts not set");
        }

        private ResourceRelationAccessor RelationAccessor(long id,
            ResourceRelationType relationType = ResourceRelationType.ParentChild,
            ResourceReferenceRole role = ResourceReferenceRole.Target,
            string relationName = null)
        {
            return new ResourceRelationAccessor
            {
                Role = role,
                Entity = Relation(id, relationType, role, relationName)
            };
        }

        [Test(Description = "Save modified references of a resource")]
        public void SaveReferences()
        {
            // Arrange
            var instance = new ReferenceResource { Id = 1 };
            _linker.SetReferenceCollections(instance);
            // Prepare reference objects
            var ref1 = new SimpleResource { Id = 2, Name = "Ref1" };
            var ref2 = new SimpleResource { Id = 3, Name = "Pos1" };
            var ref3 = new DerivedResource { Name = "Ref2" };
            _linker.SetReferenceCollections(ref3);
            var ref4 = new DerivedResource { Id = 5, Name = "ChildOnly" };
            var ref5 = new DerivedResource { Id = 6, Name = "BackRef" };
            // Set single references
            instance.Parent = ref5; // Parent remains unchanged
            instance.Reference = ref2; // Reference is changed from ref1 to ref2 
            instance.Reference2 = ref3; // Reference2 is assigned with a new object
            // Fill collections
            instance.References.Add(ref1); // This element remains
            //instance.References.Add(ref2); // This element was removed
            instance.References.Add(ref3); // The new element is also added to the list, but is not a child
            // Fill children with all except the unsaved one to simulate an unchanged collection
            instance.Children.Add(ref1);
            instance.Children.Add(ref2);
            instance.Children.Add(ref4);

            // Setup uow and repo to simulate the current database
            var relRepo = new Mock<IResourceRelationRepository>();
            var relations = new List<ResourceRelation>
            {
                // Parent child relations
                Relation(6, ResourceRelationType.ParentChild, ResourceReferenceRole.Source),
                Relation(2), Relation(3), Relation(5),
                // Current exchangable part
                Relation(2, ResourceRelationType.CurrentExchangablePart, ResourceReferenceRole.Target, nameof(ReferenceResource.Reference)), // This is changed to ref2
                // Possible exchangable part
                Relation(2, ResourceRelationType.PossibleExchangablePart), // This remains untouched
                Relation(3, ResourceRelationType.PossibleExchangablePart) // This is removed
            };
            relRepo.Setup(r => r.Linq).Returns(relations.AsQueryable());
            relRepo.Setup(r => r.Create(It.IsAny<int>())).Returns<int>(type =>
            {
                var relation = new ResourceRelation { RelationType = type };
                relations.Add(relation);
                return relation;
            });
            relRepo.Setup(r => r.RemoveRange(It.IsAny<IEnumerable<ResourceRelation>>()))
                .Callback<IEnumerable<ResourceRelation>>(removedRelations => relations.RemoveAll(removedRelations.Contains));
            var resRepo = new Mock<IResourceEntityRepository>();
            resRepo.Setup(r => r.GetByKey(It.Is<long>(id => id > 0))).Returns<long>(id => new ResourceEntity { Id = id });
            resRepo.Setup(r => r.GetByKey(It.Is<long>(id => id == 0))).Returns((ResourceEntity)null);
            resRepo.Setup(r => r.Create()).Returns(new ResourceEntity());

            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(u => u.GetRepository<IResourceRelationRepository>()).Returns(relRepo.Object);
            uowMock.Setup(u => u.GetRepository<IRepository<ResourceEntity>>()).Returns(resRepo.Object);

            // Act
            var newResources = _linker.SaveReferences(uowMock.Object, instance, new ResourceEntity { Id = 1 })
                .ToArray();

            // Assert
            Assert.AreEqual(1, newResources.Length);
            Assert.AreEqual(ref3, newResources[0]);

            Assert.DoesNotThrow(() => resRepo.Verify(repo => repo.Create(), Times.Once));
            Assert.DoesNotThrow(() => relRepo.Verify(repo => repo.Create((int)ResourceRelationType.PossibleExchangablePart), Times.Once));
            Assert.DoesNotThrow(() => relRepo.Verify(repo => repo.RemoveRange(It.Is<IEnumerable<ResourceRelation>>(removed => removed.Any())), Times.Once));

            var parentChild = relations.Where(r => r.RelationType == (int)ResourceRelationType.ParentChild).ToArray();
            Assert.AreEqual(4, parentChild.Length);
            var currentPart = relations.Where(r => r.RelationType == (int)ResourceRelationType.CurrentExchangablePart).ToArray();
            Assert.AreEqual(2, currentPart.Length);
            Assert.AreEqual(1, currentPart.Count(r => r.Target.Id == 3));
            Assert.AreEqual(1, currentPart.Count(r => r.Target.Id == 0));
            var possiblePart = relations.Where(r => r.RelationType == (int)ResourceRelationType.PossibleExchangablePart).ToArray();
            Assert.AreEqual(2, possiblePart.Length);
            Assert.AreEqual(1, possiblePart.Count(r => r.Target.Id == 2));
            Assert.AreEqual(1, possiblePart.Count(r => r.Target.Id == 0));
        }

        private ResourceRelation Relation(long id,
            ResourceRelationType relationType = ResourceRelationType.ParentChild,
            ResourceReferenceRole role = ResourceReferenceRole.Target,
            string relationName = null)
        {
            var relation = new ResourceRelation
            {
                RelationType = (int)relationType,
                RelationName = relationName,
                TargetId = role == ResourceReferenceRole.Target ? id : 1,
                SourceId = role == ResourceReferenceRole.Source ? id : 1
            };
            relation.Target = new ResourceEntity { Id = relation.TargetId };
            relation.Source = new ResourceEntity { Id = relation.SourceId };
            return relation;
        }

        [Test(Description = "A resource was deleted and should be removed from all resources referencing it")]
        public void RemoveLinking()
        {
            // Arrange
            var instance = new ReferenceResource();
            _linker.SetReferenceCollections(instance);
            var deletedRef = new DerivedResource();
            instance.Reference2 = deletedRef;
            instance.References.Add(deletedRef);
            instance.References.Add(new SimpleResource());

            // Act
            // Call once for each relation
            _linker.RemoveLinking(deletedRef, instance);
            _linker.RemoveLinking(deletedRef, instance);

            // Assert
            Assert.IsNull(instance.Reference2);
            Assert.AreEqual(1, instance.References.Count);
        }
    }
}