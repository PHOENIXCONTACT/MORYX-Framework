// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.Processes;
using Moryx.FactoryMonitor.Endpoints.Model;
using Moryx.FactoryMonitor.Endpoints.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Moryx.FactoryMonitor.Endpoints.Tests
{
    [TestFixture]

    public class FactoryMonitorController_StateStreamTest : BaseTest
    {

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            _assemblyCell.Capabilities = new DummyCapabilities1();
            _assemblyCell.Temperature = 125.2;
            _assemblyCell.Name = "Assembly 1.0";
            _assemblyCell.Parent = _manufactoringFactory;

            _solderingCell.Capabilities = new DummyCapabilities2();
            _solderingCell.Temperature = 130;
            _solderingCell.Name = "Soldering 1.0";
            _solderingCell.Parent = _manufactoringFactory;
        }

        [Test]
        public void GetInitialFactoryState()
        {
            // Arrange
            _manufactoringFactory.BackgroundUrl = backgroundUrl;

            //Act
            var endpointResult = _factoryMonitor.InitialFactoryState();

            //Assert
            Assert.That(endpointResult, Is.Not.Null);
            //number of cells in the factory
            Assert.That(GetLocations().Length, Is.EqualTo(endpointResult.Value.ResourceChangedModels.Count()));

            foreach (var endpointCell in endpointResult.Value.ResourceChangedModels)
                //machine location matches
                Assert.That(GetLocations().Any(l => l.Id == endpointCell.Location.Id));
        }

        [Test]
        public void ShouldInferCorrectCellStatus()
        {
            // Arrange
            _solderingCell.Capabilities = NullCapabilities.Instance;

            //Act
            var endpointResult = _factoryMonitor.InitialFactoryState();

            //Assert
            var cells = endpointResult.Value.CellStateChangedModels;
            var assemblyCellModel = cells.Single(c => c.Id == _assemblyCell.Id);
            Assert.That(assemblyCellModel.State, Is.EqualTo(CellState.Idle));

            var solderingCellModel = cells.Single(c => c.Id == _solderingCell.Id);
            Assert.That(solderingCellModel.State, Is.EqualTo(CellState.NotReadyToWork));
        }

        [Test]
        public void FactoryStatesStream()
        {
            //Arrange 
            var source = new CancellationTokenSource();
            var cancelToken = source.Token;
            var process = new Process
            {
                Id = 1,
                Recipe = new MyRecipe
                {
                    OrderNumber = "100000",
                    OperationNumber = "0001",
                    Classification = AbstractionLayer.Recipes.RecipeClassification.Default,
                },
            };
            var assemblyActivity = new AssemblyActivity();
            var solderingActivity = new SolderingActivity();
            var memoryStream = new MemoryStream();
            var streamResponseCells = new List<CellStateChangedModel>();
            var streamResponseOrders = new List<OrderModel>();
            var streamResponseActivities = new List<ActivityChangedModel>();

            _factoryMonitor.ControllerContext = new ControllerContext();
            _factoryMonitor.ControllerContext.HttpContext = new DefaultHttpContext();
            _factoryMonitor.ControllerContext.HttpContext.Response.Body = memoryStream;

            _processFacadeMock.Setup(pm => pm.Targets(It.IsAny<IProcess>()))
                .Returns<IProcess>(p => _activityTargets.Where(pair => pair.Key.Process == p).SelectMany(pair => pair.Value).ToList());
            _processFacadeMock.SetupGet(pm => pm.RunningProcesses).Returns(new[] { process });
            //Act

            Task.Run(async () =>
            {
                await _factoryMonitor.FactoryStatesStream(cancelToken);
            });

            //assembly activity
            StartFirstActivity(process, assemblyActivity);
            Thread.Sleep(500);
            ReadJsonData(memoryStream, streamResponseCells, streamResponseOrders, streamResponseActivities);

            // Assert
            Assert.That(streamResponseCells.LastOrDefault(x => x.Id == _assemblyCellId)?.State,
                Is.EqualTo(CellState.Running));

            //Assert part 1 
            RaiseActivityUpdated(assemblyActivity, ActivityProgress.Completed);
            Thread.Sleep(500);
            ReadJsonData(memoryStream, streamResponseCells, streamResponseOrders, streamResponseActivities);

            //verify that the assembly cell is idle
            Assert.That(streamResponseCells.LastOrDefault(x => x.Id == _assemblyCellId)?.State,
                Is.EqualTo(CellState.Idle));

            StartSecondActivity(process, solderingActivity);
            Thread.Sleep(500);
            ReadJsonData(memoryStream, streamResponseCells, streamResponseOrders, streamResponseActivities);

            //Assert part 2
            //verify that the soldering cell is running
            Assert.That(streamResponseCells.LastOrDefault(x => x.Id == _solderingCellId)?.State
                , Is.EqualTo(CellState.Running));

            RaiseActivityUpdated(solderingActivity, ActivityProgress.Completed);
            Thread.Sleep(500);
            ReadJsonData(memoryStream, streamResponseCells, streamResponseOrders, streamResponseActivities);

            //verify that the soldering cell is not running
            Assert.That(streamResponseCells.LastOrDefault(x => x.Id == _solderingCellId)?.State,
                Is.EqualTo(CellState.Idle));

            // end of the process
            RaiseProcessUpdated(process, ProcessProgress.Completed);
            Thread.Sleep(500);
            ReadJsonData(memoryStream, streamResponseCells, streamResponseOrders, streamResponseActivities);

            //Assert part 3
            _solderingCell.RaiseCapabilitiesChanged(NullCapabilities.Instance);
            Thread.Sleep(500);
            ReadJsonData(memoryStream, streamResponseCells, streamResponseOrders, streamResponseActivities);

            Assert.That(streamResponseCells.LastOrDefault(x => x.Id == _solderingCellId)?.State, Is.EqualTo(CellState.NotReadyToWork));

            //cancel/stop the request task
            memoryStream.Close();
            source.Cancel();
        }

        private void ReadJsonData(MemoryStream memoryStream, List<CellStateChangedModel> cells, List<OrderModel> orders, List<ActivityChangedModel> activities)
        {
            var jsonData = Encoding.UTF8.GetString(memoryStream.ToArray());
            if (string.IsNullOrEmpty(jsonData)) return;

            var array = jsonData.Split("type: ").ToList();
            foreach (var item in array)
            {
                if (item.Contains("cellStateChangedModel"))
                {
                    //clean up remove "cells" and "data:" text
                    var content = item.Replace("cellStateChangedModel", "").Replace("data: ", "");
                    if (!string.IsNullOrEmpty(content))
                        cells.Add(JsonConvert.DeserializeObject<CellStateChangedModel>(content));
                }
                else if (item.Contains("activityChangedModel"))
                {
                    //clean up, remove "processes" and "data:" text
                    var content = item.Replace("activityChangedModel", "").Replace("data: ", "");
                    if (!string.IsNullOrEmpty(content))
                        activities.Add(JsonConvert.DeserializeObject<ActivityChangedModel>(content));
                }
                else if (item.Contains("process"))
                {
                    //clean up, remove "processes" and "data:" text
                    var content = item.Replace("process", "").Replace("data: ", "");
                    if (!string.IsNullOrEmpty(content))
                        orders.AddRange(JsonConvert.DeserializeObject<List<OrderModel>>(content));
                }
            }
        }

        private void StartSecondActivity(Process process, SolderingActivity mySecondActivity)
        {

            // ---------------------second activity
            AssignActivity(process, mySecondActivity, _solderingCell);
            RaiseActivityUpdated(mySecondActivity, ActivityProgress.Ready);

            Thread.Sleep(200);
            //activity updated
            RaiseActivityUpdated(mySecondActivity, ActivityProgress.Running);
            RaiseProcessUpdated(process, ProcessProgress.Running);
        }

        private void StartFirstActivity(Process process, AssemblyActivity myFirstActivity)
        {
            // ----------- First activity
            AssignActivity(process, myFirstActivity, _assemblyCell);
            RaiseProcessUpdated(process, ProcessProgress.Ready);
            RaiseActivityUpdated(myFirstActivity, ActivityProgress.Ready);

            Thread.Sleep(200);

            RaiseActivityUpdated(myFirstActivity, ActivityProgress.Running);
            RaiseProcessUpdated(process, ProcessProgress.Running);
        }

        private void RaiseActivityUpdated(IActivity activity, ActivityProgress progress)
        {
            _processFacadeMock.Raise(pm => pm.ActivityUpdated += null, new ActivityUpdatedEventArgs(activity, progress));
        }

        private void RaiseProcessUpdated(IProcess process, ProcessProgress progress)
        {
            _processFacadeMock.Raise(pm => pm.ProcessUpdated += null, new ProcessUpdatedEventArgs(process, progress));
        }

        private Activity AssignActivity(IProcess process, Activity activity, ICell cell)
        {
            activity.Process = process;
            activity.Tracing.ResourceId = cell.Id;
            process.AddActivity(activity);
            // Assign resources AFTER creation
            _activityTargets[activity] = new[] { cell };
            return activity;
        }
    }
}

