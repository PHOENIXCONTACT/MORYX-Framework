// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.IO;
using System.Threading;
using Moryx.TestTools.SystemTest;
using NUnit.Framework;
using System;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.SystemTests
{
    /// <summary>
    /// These tests shall check two aspects: They shall verify the HoG functionality but also wether the HeartOfGoldController is working as expected.
    /// </summary>
    [TestFixture]
    public class BasicTests : IDisposable
    {
        private HeartOfGoldController _hogController;

        [SetUp]
        public void Setup()
        {
            _hogController = new HeartOfGoldController
            {
                RuntimeDir = HogHelper.RuntimeDir,
                ConfigDir = HogHelper.ConfigDirParam,
                ExecutionTimeout = 60
            };
        }

        [TearDown]
        public void Cleanup()
        {
            if (_hogController.Process != null && !_hogController.Process.HasExited)
            {
                Console.WriteLine("Killing HeartOfGold");
                _hogController.Process.Kill();

                Thread.Sleep(1000);

                Assert.IsTrue(_hogController.Process.HasExited, "Can't kill HeartOfGold.");
            }
        }

        public void Dispose()
        {
            if (_hogController != null)
            {
                _hogController.Dispose();
                _hogController = null;
            }
        }

        [Test]
        public void TestPath()
        {
            FileInfo fi = new FileInfo(Path.Combine(HogHelper.RuntimeDir, _hogController.ApplicationExeName));

            Assert.IsTrue(fi.Exists, "File '{0}' does not exist.", fi.FullName);
        }

        private void PrintStartMsg(string msg, params object[] args)
        {
            Console.WriteLine("{0:yyyy-MM-dd HH:mm:ss} - Starting test '{1}'", DateTime.Now, String.Format(msg, args));
        }

        [Test]
        public void TestStartStop()
        {
            PrintStartMsg("TestStartStop");

            bool started = _hogController.StartHeartOfGold();

            Assert.IsTrue(started, "Can't start HeartOfGold.");
            Assert.IsFalse(_hogController.Process.HasExited, "HeartOfGold has exited unexpectedly.");

            Thread.Sleep(3000); // Give HoG the chance to come up.

            bool stopped = _hogController.StopHeartOfGold(15);

            Assert.IsTrue(stopped, "Can't stop HeartOfGold.");

            Assert.IsTrue(_hogController.Process.HasExited, "HeartOfGold did not stop.");
        }

        [Test]
        public void TestStartTimeout()
        {
            PrintStartMsg("TestStartTimeout");

            _hogController.ExecutionTimeout = 5;
            bool started = _hogController.StartHeartOfGold();

            Assert.IsTrue(started, "Can't start HeartOfGold.");
            Assert.IsFalse(_hogController.Process.HasExited, "HeartOfGold has exited unexpectedly.");

            _hogController.Process.WaitForExit(15000);

            Assert.IsTrue(_hogController.Process.HasExited, "HeartOfGold did not stop.");
        }

        [TestCase("Maintenance", ServerModuleState.Running, Description = "Check for Maintanence to start", ExpectedResult = true)]
        [TestCase("Maintenance",  ServerModuleState.Failure, Description = "Check for Maintanence to start", ExpectedResult = false)]
        //[TestCase("OrderManager", ServerModuleState.Running, Description = "Check for OrderManager to start", ExpectedResult = false, ExpectedException = typeof(MoryxServiceNotFoundException))]
        public bool TestStartWaitStop(string service, ServerModuleState state)
        {
            PrintStartMsg("TestStartWaitStop('{0}', '{1}')", service, state);

            bool started = _hogController.StartHeartOfGold();
            _hogController.CreateClients();

            Assert.IsTrue(started, "Can't start HeartOfGold.");
            Assert.IsFalse(_hogController.Process.HasExited, "HeartOfGold has exited unexpectedly.");

            bool result = _hogController.WaitForService(service, state, 5);

            bool stopped = _hogController.StopHeartOfGold(10);

            Assert.IsTrue(stopped, "Can't stop HeartOfGold.");

            Assert.IsTrue(_hogController.Process.HasExited, "HeartOfGold did not stop.");

            return result;
        }

        [Test]
        public void TestStartKill()
        {
            PrintStartMsg("TestStartKill");

            bool started = _hogController.StartHeartOfGold();

            Assert.IsTrue(started, "Can't start HeartOfGold.");
            Assert.IsFalse(_hogController.Process.HasExited, "HeartOfGold has exited unexpectedly.");

            _hogController.KillHeartOfGold();

            Assert.IsTrue(_hogController.Process.HasExited, "Can't kill HeartOfGold.");
        }
    }
}
