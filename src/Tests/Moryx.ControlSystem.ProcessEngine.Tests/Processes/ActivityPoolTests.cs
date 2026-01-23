// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Moryx.AbstractionLayer.Processes;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.TestTools;
using NUnit.Framework;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Processes;

[TestFixture]
public class ActivityPoolTests : ProcessTestsBase
{
    [SetUp]
    public void Create()
    {
        CreateList();
    }

    [TearDown]
    public void Destory()
    {
        DestroyList();
    }

    [Test]
    public void AddProcess()
    {
        // Arrange
        var input = new ProcessData(new Process());

        // Act
        DataPool.AddProcess(input);

        // Assert
        Assert.That(ModifiedProcess, Is.Not.Null);
        Assert.That(ModifiedProcess, Is.EqualTo(input));
    }

    [TestCase(ProcessState.Running, Description = "Process started")]
    [TestCase(ProcessState.Success, Description = "Process finished")]
    public void UpdateProcess(ProcessState newState)
    {
        // Arrange
        var modified = new ProcessData(new Process());
        DataPool.AddProcess(modified);

        // Act
        DataPool.UpdateProcess(modified, newState);

        // Assert
        Assert.That(ModifiedProcess, Is.Not.Null);
        Assert.That(ModifiedProcess, Is.EqualTo(modified));
    }

    [Test]
    public void AddActivity()
    {
        // Arrange
        var proc = new ProcessData(new Process());
        DataPool.AddProcess(proc);
        var activity = new ActivityData(new DummyActivity());

        // Act
        DataPool.AddActivity(proc, activity);

        // Assert
        Assert.That(ModifiedActivity, Is.Not.Null);
        Assert.That(ModifiedActivity, Is.EqualTo(activity));
        Assert.That(proc.Activities.Count, Is.EqualTo(1));
        Assert.That(proc.Activities.First(), Is.EqualTo(activity));
    }

    [Test]
    public void UpdateActivity()
    {
        // Arrange
        var proc = new ProcessData(new Process());
        DataPool.AddProcess(proc);
        var activity = new ActivityData(new DummyActivity());
        DataPool.AddActivity(proc, activity);

        // Act
        DataPool.TryUpdateActivity(activity, ActivityState.Configured);

        // Assert
        Assert.That(activity, Is.Not.Null);
        Assert.That(ModifiedActivity, Is.EqualTo(activity));
    }

    [TestCase(true, true, Description = "Look for data of an existing activity")]
    [TestCase(true, false, Description = "Look for data of an existing resource id")]
    [TestCase(false, true, Description = "Look for data of unknown activity")]
    [TestCase(false, false, Description = "Look for data of unknown resource id")]
    public void GetData(bool exists, bool byInstance)
    {
        // Arrange
        var activity = new DummyActivity() { Id = 42 };
        var proc = new ProcessData(new Process() { Id = 1337 });
        var data = new ActivityData(activity) { Resource = new CellReference(42) };

        DataPool.AddProcess(proc);
        DataPool.AddActivity(proc, data);
        DataPool.TryUpdateActivity(data, ActivityState.Configured);

        // Act
        var procMatch = DataPool.GetProcess(1337);
        var match = byInstance
            ? DataPool.GetByActivity(exists ? activity : new DummyActivity())
            : DataPool.GetByCondition(ad => ad.Resource.Id == (exists ? 42 : 0815)).FirstOrDefault();

        // Assert
        Assert.That(procMatch, Is.Not.Null);
        Assert.That(procMatch, Is.EqualTo(proc));

        if (exists)
        {
            Assert.That(match, Is.Not.Null);
            Assert.That(match, Is.EqualTo(data));
        }
        else
        {
            Assert.That(match, Is.Null);
        }
    }

    [Test]
    public void FillPool()
    {
        // Arrange: Create a backup of 3 processes with 1 activity each
        var processes = new ProcessData[3];
        var activities = new ActivityData[3];
        for (var i = 0; i < 3; i++)
        {
            processes[i] = new ProcessData(new ProductionProcess { Id = i + 1 });
            activities[i] = new ActivityData(new DummyActivity()) { Resource = new CellReference((i + 1) * 10) };

            DataPool.AddProcess(processes[i]);
            DataPool.AddActivity(processes[i], activities[i]);
        }

        // Act: Try getting instances
        var match1 = DataPool.GetByCondition(ad => ad.Resource.Id == 10);
        var match2 = DataPool.GetByActivity(activities[1].Activity);

        // Assert
        Assert.That(match1, Is.Not.Null);
        Assert.That(match2, Is.Not.Null);
    }

    [Test(Description = "20 Threads randomly read and write on the pool with a share of 90/10")]
    public void ThreadSafe()
    {
        // Arrange: 
        var contexts = new ThreadContext[20];
        for (var i = 0; i < 20; i++)
        {
            contexts[i] = new ThreadContext { Run = true };
            ThreadPool.QueueUserWorkItem(DummyThread, contexts[i]);
        }

        // Act: Wait for 5s if any of the threads crash
        Thread.Sleep(5000);
        foreach (var context in contexts)
        {
            context.Run = false;
        }
        Thread.Sleep(1); // Let them finish

        // Some feedback
        Console.WriteLine("Write operations: {0}", contexts.Sum(c => c.WriteCount));
        Console.WriteLine("Read operations: {0}", contexts.Sum(c => c.ReadCount));

        // Assert
        Assert.That(contexts.All(c => c.Failed == false));
    }

    private class ThreadContext
    {
        public bool Run { get; set; }

        public bool Failed { get; set; }

        public int WriteCount { get; set; }

        public int ReadCount { get; set; }
    }

    /// <summary>
    /// Number of processes in the pool
    /// </summary>
    private readonly IList<ProcessData> _allProcesses = new List<ProcessData>();

    /// <summary>
    /// Single thread that randomly writes or reads with a share of 90/10
    /// </summary>
    private void DummyThread(object contextObj)
    {
        var random = new Random();
        var context = (ThreadContext)contextObj;
        var myProcesses = new List<ProcessData>();

        try
        {
            while (context.Run)
            {
                // Only 1 in 10 runs shall write, all others read
                var choice = random.Next(100);
                if (choice % 10 == 9)
                {
                    // Keep a stable amount of 20 processes
                    int procCount;
                    lock (_allProcesses)
                        procCount = _allProcesses.Count;

                    if (procCount < 20)
                    {
                        var process = new ProcessData(new ProductionProcess());
                        process.AddActivity(new ActivityData(new DummyActivity()) { Resource = new CellReference(random.Next(10)) });

                        myProcesses.Add(process);
                        DataPool.AddProcess(process);
                    }
                    // Remove if we can remove something
                    else if (myProcesses.Count >= 1)
                    {
                        var process = myProcesses[0];
                        myProcesses.Remove(process);

                        DataPool.UpdateProcess(process, ProcessState.Success);
                    }
                    // Otherwise use the timeslice to add activity to some process
                    else
                    {
                        var index = random.Next(procCount - 5);
                        ProcessData someProcess;
                        lock (_allProcesses)
                            someProcess = _allProcesses[index];
                        DataPool.AddActivity(someProcess, new ActivityData(new DummyActivity()));
                    }

                    context.WriteCount++;
                }
                else
                {
                    // Try reading some activity
                    DataPool.GetByCondition(ad => ad.Resource.Id == random.Next(10));
                    context.ReadCount++;
                }

                Thread.Sleep(1); // Context switch between threads
            }
        }
        catch (Exception)
        {
            context.Failed = true;
        }
    }
}