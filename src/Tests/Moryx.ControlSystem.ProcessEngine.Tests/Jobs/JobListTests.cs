// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.ControlSystem.TestTools;
using Moryx.Logging;
using NUnit.Framework;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Jobs
{
    [TestFixture]
    public class JobListTests
    {
        private JobList _jobList;

        private IEnumerable<IJobData> _receivedJobAdded;
        private IJobData _receivedStateChanged;
        private JobStateEventArgs _receivedDetailedStateChanged;
        private IJobData _recievedJobCompleted;
        private IJobData _recievedJobProgressChanged;
        private IProductRecipe _recipe;
        private List<IJobData> _storedJobs;

        [SetUp]
        public void TestFixtureSetUp()
        {
            var recipeProviderMock = new Mock<IRecipeProvider>();

            _recipe = new DummyRecipe { Id = 42, Origin = recipeProviderMock.Object };

            recipeProviderMock.Setup(r => r.LoadRecipe(_recipe.Id)).Returns(_recipe);
            recipeProviderMock.SetupGet(r => r.Name).Returns("RecipeProviderMock");
        }

        [SetUp]
        public void SetUp()
        {
            _storedJobs = new List<IJobData>();
            _jobList = new JobList
            {
                Logger = new ModuleLogger("Dummy", new NullLoggerFactory())
            };

            _jobList.Start();

            _jobList.Added += delegate (object sender, IReadOnlyList<IJobData> jobDatas)
            {
                _receivedJobAdded = jobDatas;
            };

            _jobList.StateChanged += delegate (object sender, JobStateEventArgs args)
            {
                if (args.CurrentState.Classification < JobClassification.Completed)
                    _receivedStateChanged = args.JobData;
                else
                    _recievedJobCompleted = args.JobData;
            };
            _jobList.StateChanged += delegate (object sender, JobStateEventArgs eventArgs)
                {
                    _receivedDetailedStateChanged = eventArgs;
                };

            _jobList.ProgressChanged += delegate (object sender, IJobData jobData)
            {
                _recievedJobProgressChanged = jobData;
            };

            var prefillList = new List<IJobData>();
            for (int i = 0; i < 10; i++)
            {
                var jobData = CreateJobMock(i + 1);
                prefillList.Add(jobData);
            }
            _jobList.Restore(prefillList, StorageCallback);

            _storedJobs.Clear();

            _receivedJobAdded = null;
            _receivedStateChanged = null;
            _recievedJobCompleted = null;
            _recievedJobProgressChanged = null;
        }

        private void StorageCallback(ModifiedJobsFragment modifiedJobs) => _storedJobs.AddRange(modifiedJobs.AffectedJobs);

        [Test(Description = "Iterates the list in forward direction")]
        public void ForwardIteration()
        {
            // Arrange
            var enumerator = _jobList.GetEnumerator();

            // Act
            for (int i = 0; i < 10; i++)
            {
                enumerator.MoveNext();
                var next = enumerator.Current;

                // Assert
                Assert.That(next, Is.Not.Null);
                Assert.That(next.Id, Is.EqualTo(i + 1));
            }

            // Cleanup
            enumerator.Dispose();
        }

        [Test(Description = "Iterates the list in forward direction with from the next to last job as the start point")]
        public void ForwardWithStartIteration()
        {
            // Arange
            var nextToLast = _jobList.Get(9);
            var last = _jobList.Get(10);
            var enumerator = _jobList.Forward(nextToLast).GetEnumerator();

            // Act
            enumerator.MoveNext();

            // Assert
            // Next should be the lastl in the list
            Assert.That(enumerator.Current, Is.Not.Null);
            Assert.That(enumerator.Current, Is.EqualTo(last));

            // Additional iteration should be null
            enumerator.MoveNext();
            Assert.That(enumerator.Current, Is.Null);

            // Cleanup
            enumerator.Dispose();
        }

        [Test(Description = "Iterates the list in backward direction")]
        public void BackwardIteration()
        {
            // Arange
            var enumerator = _jobList.Backward().GetEnumerator();

            // Act
            for (int i = 10; i > 0; i--)
            {
                enumerator.MoveNext();
                var next = enumerator.Current;

                // Assert
                Assert.That(next, Is.Not.Null);
                Assert.That(next.Id, Is.EqualTo(i));
            }

            // Cleanup
            enumerator.Dispose();
        }

        [Test(Description = "Iterates the list in backward direction with the second job as the start point")]
        public void BackwardWithStartIteration()
        {
            // Arange
            var nextToFirst = _jobList.Get(2);
            var first = _jobList.Get(1);
            var enumerator = _jobList.Backward(nextToFirst).GetEnumerator();

            // Act
            enumerator.MoveNext();

            // Assert
            // Next should be the first in the list
            Assert.That(enumerator.Current, Is.Not.Null);
            Assert.That(enumerator.Current, Is.EqualTo(first));

            // Additional iteration should be null
            enumerator.MoveNext();
            Assert.That(enumerator.Current, Is.Null);

            // Cleanup
            enumerator.Dispose();
        }

        [Test(Description = "Loads a job by the id from the job list.")]
        public void GetJobById()
        {
            // Arrange
            var firstJob = _jobList.First();

            // Act
            var jobByGet = _jobList.Get(firstJob.Id);

            // Assert
            Assert.That(jobByGet, Is.Not.Null, "No job found");
            Assert.That(jobByGet, Is.EqualTo(firstJob), "Different jobs");
        }

        [Test(Description = "Trys to load a job with an unknown job which should return a null because this could be an old completed job")]
        public void GetJobByAnUnknownId()
        {
            // Act
            var unknownJob = _jobList.Get(int.MaxValue);

            // Assert
            Assert.That(unknownJob, Is.Null, "No job was expected");
        }

        [Test(Description = "Adds a single job to the job list. Events should be published and job should be raised to store.")]
        public void AppendSingleJob()
        {
            // Arrange
            var jobData = CreateJobMock();
            var linkedList = new LinkedList<IJobData>(new[] { jobData });

            // Act
            _jobList.Add(linkedList, JobPosition.Append, StorageCallback);

            // Assert
            // Check storage event
            Assert.That(_storedJobs.Count, Is.EqualTo(linkedList.Count));

            // Compare return value and event
            Assert.That(_receivedJobAdded.Count(), Is.EqualTo(linkedList.Count));

            var addedJob = linkedList.Single();
            Assert.That(_receivedJobAdded.Single(), Is.EqualTo(addedJob));

            // Check if job was Append to the list
            var lastJob = _jobList.Backward().First();
            Assert.That(addedJob, Is.EqualTo(lastJob));
        }

        [Test(Description = "Adds a job with AfterOther")]
        public void AddJobAfterOther()
        {
            // Arrange
            var addedJob = CreateJobMock(42);
            var otherJob = _jobList.Get(4);
            var linkedList = new LinkedList<IJobData>(new[] { addedJob });

            // Act
            var position = new JobPosition(JobPositionType.AfterOther, otherJob.Id);
            _jobList.Add(linkedList, position, StorageCallback);

            // Assert
            // The next of the other job should now our new job
            var next = _jobList.Forward(otherJob).First();
            Assert.That(addedJob, Is.EqualTo(next));

            // The old following job should now be the next of our added job
            var oldNext = _jobList.Forward(addedJob).First();
            var previous = _jobList.Backward(addedJob).First();

            // The sort orders should show the changes
            Assert.That(oldNext.Id, Is.EqualTo(5));
            Assert.That(previous.Id, Is.EqualTo(4));
        }

        [Test(Description = "A mixture of new and existing jobs is inserted")]
        public void ExpandAroundExisting()
        {
            // Arrange: Mix new jobs into exisiting ones
            var newList = new LinkedList<IJobData>(new List<IJobData>
            {
                CreateJobMock(), // One before existing
                _jobList.Get(3),
                CreateJobMock(), // Two between follow-up jobs
                CreateJobMock(),
                _jobList.Get(4),
                CreateJobMock(), // One new one
                _jobList.Get(7), // Skipping 5 & 6
                CreateJobMock()
            });

            // Act: Insert into job list
            Assert.DoesNotThrow(() => _jobList.Add(newList, JobPosition.Expand, StorageCallback));
            Assert.That(_jobList.Count(), Is.EqualTo(15));
        }

        [Test(Description = "Automatically add ")]
        public void AppendToRecipe()
        {
            // Arrange: Add follow-up to job 3
            var job3 = _jobList.Get(3);
            var followUps = new LinkedList<IJobData>(new[]
            {
                CreateJobMock(42, 3),
                CreateJobMock(43, 3)
            });
            _jobList.Add(followUps, new JobPosition(JobPositionType.AfterOther, 3), StorageCallback);

            // Act: Append to recipe
            var appendRecipe = new LinkedList<IJobData>(new[] { CreateJobMock(100, 3) });
            _jobList.Add(appendRecipe, new JobPosition(JobPositionType.AppendToRecipe, 3), StorageCallback);

            // Assert: Make sure the recipe was placed behind the last job of that recipe
            Assert.That(_jobList.Count(), Is.EqualTo(13));
            var last = _jobList.Get(43);
            Assert.That(42, Is.EqualTo(_jobList.Next(job3).Id));
            Assert.That(42, Is.EqualTo(_jobList.Previous(last).Id));
            Assert.That(100, Is.EqualTo(_jobList.Next(last).Id));
        }

        [Test(Description = "A jobs updated event must be raised if the job changes its state")]
        public void JobStateChange()
        {
            // Arrange
            var jobData = _jobList.First();

            // Act
            RaiseStateChanged(jobData, JobClassification.Waiting);

            // Assert
            Assert.That(_receivedStateChanged, Is.EqualTo(jobData));
            Assert.That(_receivedDetailedStateChanged, Is.Not.Null);
            Assert.That(_receivedDetailedStateChanged.JobData, Is.EqualTo(jobData));
            Assert.That(_receivedDetailedStateChanged.CurrentState.Classification, Is.EqualTo(JobClassification.Waiting));

            var changedJob = _receivedStateChanged;
            Assert.That(changedJob, Is.EqualTo(jobData), "Unexpected value of jobData");
        }

        [Test(Description = "A jobs progress changed event must be raised if the job progress changed")]
        public void JobProgressChanged()
        {
            // Arrange
            var jobData = _jobList.First();

            // Act
            RaiseProgressChanged(jobData);

            // Assert
            Assert.That(_recievedJobProgressChanged, Is.Not.Null);
            Assert.That(_recievedJobProgressChanged, Is.EqualTo(jobData));
        }

        [Test(Description = "A job must be removed from the job list if the job was completed")]
        public void JobRemovedOnComplete()
        {
            // Arrange
            var jobData = _jobList.Skip(2).First();

            // Act
            RaiseStateChanged(jobData, JobClassification.Completed);
            ModifiedJobsFragment fragment = null;
            _jobList.Remove(jobData, f => fragment = f);


            // Assert
            Assert.That(_recievedJobCompleted, Is.EqualTo(jobData));
            Assert.That(fragment, Is.Not.Null);
            Assert.That(fragment.PreviousId, Is.EqualTo(jobData.Id - 1));
            Assert.That(fragment.AffectedJobs.Count, Is.EqualTo(2));
            Assert.That(fragment.AffectedJobs[0], Is.EqualTo(jobData));
            Assert.That(fragment.AffectedJobs[1].Id, Is.EqualTo(jobData.Id + 1));
        }

        private static IJobData CreateJobMock(long id = 0, long recipeId = 0)
        {
            var jobMock = new Mock<IJobData>();
            jobMock.SetupGet(j => j.Id).Returns(id);
            if (recipeId == 0 && id > 0)
                recipeId = id;
            jobMock.SetupGet(j => j.Recipe).Returns(new ProductionRecipe { Id = recipeId });
            var jobStateMock = new Mock<IJobState>();
            jobStateMock.SetupGet(s => s.Classification).Returns(JobClassification.Idle);
            jobMock.SetupGet(j => j.State).Returns(jobStateMock.Object);

            return jobMock.Object;
        }

        private static void RaiseStateChanged(IJobData jobData, JobClassification classification)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            var jobDataMock = (Mock<IJobData>)((IMocked)jobData).Mock;
            var oldState = jobData.State;
            var jobStateMock = new Mock<IJobState>();
            jobStateMock.SetupGet(s => s.Classification).Returns(classification);
            jobDataMock.SetupGet(j => j.State).Returns(jobStateMock.Object);

            jobDataMock.Raise(j => j.StateChanged += null, jobData, new JobStateEventArgs(jobData, oldState, jobStateMock.Object));
        }

        private static void RaiseProgressChanged(IJobData jobData)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            var jobDataMock = (Mock<IJobData>)((IMocked)jobData).Mock;
            jobDataMock.Raise(j => j.ProgressChanged += null, jobData, EventArgs.Empty);
        }
    }
}
