using System;
using System.IO;
using System.Linq;
using System.Threading;
using Marvin.Runtime.Base.TaskBase;
using Marvin.Runtime.Kernel.Configuration;
using Marvin.Runtime.Kernel.TaskManagement;
using Marvin.Runtime.Kernel.Tests.Mocks;
using NUnit.Framework;

namespace Marvin.Runtime.Kernel.Tests
{
    /// <summary>
    /// Tests for the taskmanager and the schedulers
    /// </summary>
    [TestFixture]
    public class TaskManagerTests
    {
        private readonly string _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        private RuntimeConfigManager _configManager;
        private TaskManager _taskManager;

        /// <summary>
        /// Setup for the test
        /// </summary>
        [OneTimeSetUp]
        public void Setup()
        {
            Directory.CreateDirectory(_tempDirectory);

            _configManager = new RuntimeConfigManager()
            {
                ConfigDirectory = _tempDirectory
            };

            var config = new TaskManagerConfig()
            {
                ThreadCount = 42
            };
            _configManager.SaveConfiguration(config);
          
            _taskManager = new TaskManager
            {
                ConfigManager = _configManager,
                Logging = new LoggerManagementMock()
            };

            _taskManager.Initialize();
            Assert.AreEqual(_taskManager.WorkerThreads, config.ThreadCount);
        }


        /// <summary>
        /// Test for the SingleExecutionSchedule.
        /// Check for correct start time of the task.
        /// </summary>
        /// <param name="timeOffset">The time offset in seconds.</param>
        /// <param name="executeExpected">if set to <c>true</c> [execute expected].</param>
        [TestCase(-1.0, true, Description = "Start one second ago: Tests if the schedule starts imediatly.")]
        [TestCase(0.0, true, Description = "Start now. Tests if the schedule starts imediatly.")]
        [TestCase(0.5, true, Description = "Start in half a second: Test if the schedule waits to start.")]
        [TestCase(1.1, false, Description = "Start in one second: Test if the schedule waits to start.")]
        public void SingleExecutionScheduleTest(double timeOffset, bool executeExpected)
        {
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            var task = new TaskMock();

            task.ExecutingCallback = delegate
            {
                manualResetEvent.Set(); // response from task
            };

            // add (or substract) the timer offset to the NOW-time
            var time = DateTime.Now.AddSeconds(timeOffset);

            // create a schedule
            var schedule = new SingleExecutionSchedule(time);
            // check that it is not executed now
            Assert.IsFalse(task.Executed, "Task is allready executed!");
            Assert.IsTrue(schedule.Executable, "Schedule is not executable!");
            // start the schedule
            _taskManager.Schedule(task, schedule);
            // wait max. 1s  to get a response from the task
            manualResetEvent.WaitOne(1000);
            // check that the response is as expected
            Assert.AreEqual(executeExpected, task.Executed, "Execution result is not like expected!");
            // Wait 10ms to give the manager the chance to set the schedule to not executable.
            Thread.Sleep(10);

            if (executeExpected)
                Assert.IsFalse(schedule.Executable, "Schedule is still executable!");
            else
                Assert.IsTrue(schedule.Executable, "Schedule should be still executable!");
        }

        /// <summary>
        /// Test for the FixedTimeSchedule.
        /// Check for correct start time of the task.
        /// </summary>
        /// <param name="timeOffset">The time offset in minutes.</param>
        /// <param name="executeExpected">if set to <c>true</c> [execute expected].</param>
        [TestCase(-2.0, true, Description = "Start two minutes in the past.")]      // TODO: Discuss required behaviour for time stamps in the past.
        [TestCase(-1.0, true, Description = "Start one minute in the past.")]      // TODO: Discuss required behaviour for time stamps in the past.
        [TestCase(0.0, true, Description = "Start right now.")]
        //[TestCase(1.0, true, Description = "Start after one minute. (This test will need 1 minute!)")]    TODO: Fix postponed until first usage.
        //[TestCase(2.0, false, Description = "Start after two minutes. (This test will need 1 minute!)")]    TODO: Fix postponed until first usage.
        public void FixedTimeScheduleTest(double timeOffset, bool executeExpected)
        {
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            var task = new TaskMock();

            task.ExecutingCallback = delegate
            {
                manualResetEvent.Set(); // response from the task
            };

            // get start time
            var time = DateTime.Now.AddSeconds(timeOffset);
            // create schedule
            var schedule = new FixedTimeSchedule(time.Hour, time.Minute, time.Second);
            
            Assert.IsFalse(task.Executed, "Task is allready executed!");

            // start schedule
            _taskManager.Schedule(task, schedule);
            // wait up to one minute or until a response from the task
            manualResetEvent.WaitOne(1500);
            // check if it was executed or not
            Assert.AreEqual(executeExpected, task.Executed, "Execution result is not like expected!");
        }

        /// <summary>
        /// Test for the IntervalExecutionSchedule.
        /// Check for the cyclic execution.
        /// </summary>
        [Test(Description = "Check interval")]
        public void IntervalExecutionScheduleTest()
        {
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            var task = new TaskMock();

            task.ExecutingCallback = delegate
            {
                manualResetEvent.Set();
            };

            // fixed interval
            var time = new TimeSpan(0, 0, 0, 0, 100);
            // create a schedule
            var schedule = new IntervalExecutionSchedule(time);

            Assert.IsFalse(task.Executed, "Task is allready executed!");
            // start schedule
            _taskManager.Schedule(task, schedule);

            manualResetEvent.WaitOne(1000);

            // check if task has run
            Assert.IsTrue(task.Executed);

            // check number of task cycles
            var firstCount = task.ExecutionCount;
            Assert.GreaterOrEqual(firstCount, 1, "The task was not executed!");

            manualResetEvent.Reset();
            manualResetEvent.WaitOne(1000);
            
            // check for a second cycle
            Assert.Greater(task.ExecutionCount, firstCount, "The task was not executed in an interval."); 
        }

        /// <summary>
        /// Test for the IntervalExecutionSchedule.
        /// Check for the cyclic execution and the correct start time.
        /// </summary>
        /// <param name="startOffset">The start offset in seconds.</param>
        /// <param name="expectedExecution">The expected number of execution cycles.</param>
        [TestCase(-1.0, 2, Description = "Start one second ago.")]
        [TestCase(0.0, 2, Description = "Start right now!")]
        [TestCase(1.0, 1, Description = "Wer das liest ist doof!")]
        [TestCase(2.0, 0, Description = "Start in two seconds.")]
        public void IntervalExecutionScheduleTest(double startOffset, int expectedExecution)
        {
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            var task = new TaskMock();

            task.ExecutingCallback = delegate
            {
                manualResetEvent.Set(); // response from the task
            };

            var time = new TimeSpan(0, 0, 0, 0, 100);
            var startTime = DateTime.Now.AddSeconds(startOffset);
            // create schedule
            var schedule = new IntervalExecutionSchedule(time, startTime);

            Assert.AreEqual(0, task.ExecutionCount, "Initial execution count is not zero.");
            // start schedule
            _taskManager.Schedule(task, schedule);

            // wait for two cycles
            manualResetEvent.WaitOne(1000);
            manualResetEvent.Reset();
            manualResetEvent.WaitOne(1000);
            
            Assert.AreEqual(expectedExecution, task.ExecutionCount, "We do not get the correct number of executions!" );
        }

        /// <summary>
        /// Test for the IntervalExecutionSchedule.
        /// Check for the cyclic execution and the correct start time.
        /// </summary>
        /// <param name="startOffset">The start time offset in seconds.</param>
        /// <param name="dueOffset">The offset to the due time in seconds.</param>
        /// <param name="expectedExecution">The expected number of execution cycles.</param>
        //ToDo: How can we test this?
        //[TestCase(-1.0, 0.0, 1000, 2, Description = "Start was one second ago.")]
        //[TestCase(-1.0, 0.5, 1000, 2, Description = "Start was one second ago")]
        //[TestCase(-1.0, 1.0, 1000, 2, Description = "Start was one second ago")]
        //[TestCase(0.0, 0.0, 1000, 2, Description = "Start now")]
        //[TestCase(0.0, 0.5, 1000, 2, Description = "Start now")]
        //[TestCase(0.0, 1.0, 1000, 1, Description = "Start now")]
        //[TestCase(1.0, 0.0, 1000, 1, Description = "Start in one second")]
        //[TestCase(1.0, 0.5, 1000, 1, Description = "Start in one second")]
        //[TestCase(1.0, 1.0, 1000, 1, Description = "Start in one second")]
        //[TestCase(2.0, 0.0, 1000, 0, Description = "Start in two seconds")]
        //[TestCase(2.0, 0.5, 1000, 0, Description = "Start in two seconds")]
        //[TestCase(2.0, 1.0, 1000, 0, Description = "Start in two seconds")]
        public void IntervalExecutionScheduleTest(double startOffset, double dueOffset, int taskTime, int expectedExecution)
        {
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            var task = new TaskMock();

            task.ExecutingCallback = delegate
            {
                Thread.Sleep(taskTime);
                manualResetEvent.Set(); // response of the task
            };

            var time = new TimeSpan(0, 0, 0, 0, 100);
            var now = DateTime.Now;
            var startTime = now.AddSeconds(startOffset);
            var dueTime = now.AddSeconds(dueOffset);
            // create schedule
            var schedule = new IntervalExecutionSchedule(time, startTime, dueTime);
            
            Assert.AreEqual(0, task.ExecutionCount,  "The initial execution count is not zero!");

            // start schedule
            _taskManager.Schedule(task, schedule);

            // wait two cycles
            manualResetEvent.WaitOne(1000);

            Assert.AreEqual(expectedExecution, task.ExecutionCount, "We do not get the correct number of executions!");
        }


        /// <summary>
        /// Tests the CombinedSchedule.
        /// Checks if a sub schedule is called.
        /// </summary>
        //ToDo: Thomas, please explane the combined schedule function!
        //[Test(Description = "Check if a combined schedule executes all schedules.")]
        public void CombinedScheduletest()
        {
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            var task = new TaskMock();

            task.ExecutingCallback = delegate
            {
                manualResetEvent.Set(); // response of the task
            };

            var now = DateTime.Now;

            var fixedSchedule = new FixedTimeSchedule(now.AddMinutes(-1.0).Minute);
            var singelSchedule = new SingleExecutionSchedule(now.AddMilliseconds(400));
            var intervalSchedule = new IntervalExecutionSchedule(new TimeSpan(0,0,0,0,800), now.AddMilliseconds(800));
            var subCombinedSchedule = new CombinedSchedule(fixedSchedule, intervalSchedule, singelSchedule);

            var schedule = new CombinedSchedule(subCombinedSchedule);

            Assert.AreEqual(0, task.ExecutionCount, "Initial execution count is not zero.");

            _taskManager.Schedule(task, schedule);

            // wait two cycles
            manualResetEvent.WaitOne(1000); 
            Assert.IsFalse(singelSchedule.Executable);

            Assert.AreEqual(3, task.ExecutionCount, "We do not get the correct number of executions!");
        }


        /// <summary>
        /// Identifies a task by its id and delete task by disposing it.
        /// </summary>
        [Test(Description = "Identifies a task by its id and delete task by disposing it.")]
        public void IdentifyAndDeleteTask()
        {
            var time = new TimeSpan(0, 0, 0, 0, 100);
            var schedule = new IntervalExecutionSchedule(time);
            
            // create three tasks
            var task = new TaskMock(); 
            task.Name = Guid.NewGuid().ToString();
            _taskManager.Schedule(task, schedule);

            var taskToFind = new TaskMock();
            taskToFind.Name = Guid.NewGuid().ToString();
            var expectedName = taskToFind.Name;
            var id = _taskManager.Schedule(taskToFind, schedule);

            task = new TaskMock();
            task.Name = Guid.NewGuid().ToString();
            _taskManager.Schedule(task, schedule);

            // try to find the second by its id
            var found = _taskManager.Tasks.FirstOrDefault(scheduledTask => scheduledTask.Id == id);
            
            Assert.NotNull(found, "Could not find task by id.");
            Assert.AreEqual(expectedName, found.Name, "Task name differs from the expected name.");

            taskToFind.Dispose(); // dispose of task should remove it from task list

            found = _taskManager.Tasks.FirstOrDefault(scheduledTask => scheduledTask.Id == id);
           
            Assert.Null(found, "Task is still in list.");
        }

        /// <summary>
        /// Shuts down the test.
        /// </summary>
        [OneTimeTearDown]
        public void TearDown()
        {
            if (Directory.Exists(_tempDirectory))
                Directory.Delete(_tempDirectory, true);
        }
    }
}
