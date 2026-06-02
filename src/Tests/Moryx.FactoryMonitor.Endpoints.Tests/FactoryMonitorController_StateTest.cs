// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.Processes;
using Moryx.FactoryMonitor.Endpoints.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Processes;
using Microsoft.Extensions.DependencyInjection;

namespace Moryx.FactoryMonitor.Endpoints.Tests;

[TestFixture]
public class FactoryMonitorController_StateStreamTest : BaseTest
{
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    [SetUp]
    public override void Setup()
    {
        base.Setup();

        _assemblyCell.ChangeCapabilities(new DummyCapabilities1());
        _assemblyCell.Temperature = 125.2;
        _assemblyCell.Name = "Assembly 1.0";
        _assemblyCell.Parent = _manufactoringFactory;

        _solderingCell.ChangeCapabilities(new DummyCapabilities2());
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
        Assert.That(GetLocations().Length, Is.EqualTo(endpointResult.Value.ResourceChangedModels.Count));

        foreach (var endpointCell in endpointResult.Value.ResourceChangedModels)
            //machine location matches
            Assert.That(GetLocations().Any(l => l.Id == endpointCell.Location.Id));
    }

    [Test]
    public void ShouldInferCorrectCellStatus()
    {
        // Arrange
        _solderingCell.ChangeCapabilities(NullCapabilities.Instance);

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
    public async Task FactoryStatesStream()
    {
        //Arrange
        var source = new CancellationTokenSource();
        var cancellationToken = source.Token;
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
        _factoryMonitor.ControllerContext.HttpContext.RequestServices = new ServiceCollection()
            .Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(o =>
            {
                o.SerializerOptions.PropertyNamingPolicy = _serializerOptions.PropertyNamingPolicy;
                foreach (var c in _serializerOptions.Converters)
                    o.SerializerOptions.Converters.Add(c);
            })
            .BuildServiceProvider();

        _factoryMonitor.ControllerContext.HttpContext.Response.Body = memoryStream;

        _processFacadeMock.Setup(pm => pm.Targets(It.IsAny<Process>()))
            .Returns<Process>(p => _activityTargets.Where(pair => pair.Key.Process == p).SelectMany(pair => pair.Value).ToList());
        _processFacadeMock.Setup(pm => pm.GetRunningProcesses()).Returns([process]);

        // Act — event handlers and the channel subscriber are set up synchronously before
        // FactoryStatesStream suspends at its first await
        var streamTask = _factoryMonitor.FactoryStatesStream(cancellationToken);

        //assembly activity
        await StartFirstActivityAsync(process, assemblyActivity);
        await Task.Delay(500);
        ReadJsonData(memoryStream, streamResponseCells, streamResponseOrders, streamResponseActivities);

        // Assert
        Assert.That(streamResponseCells.LastOrDefault(x => x.Id == _assemblyCellId)?.State,
            Is.EqualTo(CellState.Running));

        //Assert part 1
        RaiseActivityUpdated(assemblyActivity, ActivityProgress.Completed);
        await Task.Delay(500);
        ReadJsonData(memoryStream, streamResponseCells, streamResponseOrders, streamResponseActivities);

        //verify that the assembly cell is idle
        Assert.That(streamResponseCells.LastOrDefault(x => x.Id == _assemblyCellId)?.State,
            Is.EqualTo(CellState.Idle));

        await StartSecondActivityAsync(process, solderingActivity);
        await Task.Delay(500);
        ReadJsonData(memoryStream, streamResponseCells, streamResponseOrders, streamResponseActivities);

        //Assert part 2
        //verify that the soldering cell is running
        Assert.That(streamResponseCells.LastOrDefault(x => x.Id == _solderingCellId)?.State,
            Is.EqualTo(CellState.Running));

        RaiseActivityUpdated(solderingActivity, ActivityProgress.Completed);
        await Task.Delay(500);
        ReadJsonData(memoryStream, streamResponseCells, streamResponseOrders, streamResponseActivities);

        //verify that the soldering cell is not running
        Assert.That(streamResponseCells.LastOrDefault(x => x.Id == _solderingCellId)?.State,
            Is.EqualTo(CellState.Idle));

        // end of the process
        RaiseProcessUpdated(process, ProcessProgress.Completed);
        await Task.Delay(500);
        ReadJsonData(memoryStream, streamResponseCells, streamResponseOrders, streamResponseActivities);

        //Assert part 3
        _solderingCell.ChangeCapabilities(NullCapabilities.Instance);
        await Task.Delay(500);
        ReadJsonData(memoryStream, streamResponseCells, streamResponseOrders, streamResponseActivities);

        Assert.That(streamResponseCells.LastOrDefault(x => x.Id == _solderingCellId)?.State, Is.EqualTo(CellState.NotReadyToWork));

        // Cleanup
        // Cancel first — the channel's ReadAllAsync then throws OperationCanceledException cleanly.
        // Await the task so the controller's finally block finishes before the test returns.
        source.Cancel();
        try
        {
            await streamTask;
        }
        catch (OperationCanceledException)
        {
        }
    }

    private static void ReadJsonData(MemoryStream memoryStream, List<CellStateChangedModel> cells, List<OrderModel> orders, List<ActivityChangedModel> activities)
    {
        var text = Encoding.UTF8.GetString(memoryStream.ToArray());
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        // SSE events are delimited by blank lines
        foreach (var block in text.Split(["\n\n", "\r\n\r\n"], StringSplitOptions.RemoveEmptyEntries))
        {
            string eventType = null;
            string data = null;

            foreach (var line in block.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.StartsWith("event: "))
                {
                    eventType = line["event: ".Length..];
                }
                else if (line.StartsWith("data: "))
                {
                    data = line["data: ".Length..];
                }
            }

            if (data == null)
            {
                continue;
            }

            switch (eventType)
            {
                case "cellStateChangedModel":
                    cells.Add(JsonSerializer.Deserialize<CellStateChangedModel>(data, _serializerOptions));
                    break;
                case "activityChangedModel":
                    activities.Add(JsonSerializer.Deserialize<ActivityChangedModel>(data, _serializerOptions));
                    break;
                case "processes":
                    var order = JsonSerializer.Deserialize<OrderModel>(data, _serializerOptions);
                    if (order != null) orders.Add(order);
                    break;
                // resourceChangedModel is intentionally ignored in these tests
            }
        }
    }

    private async Task StartSecondActivityAsync(Process process, SolderingActivity mySecondActivity)
    {
        // ---------------------second activity
        AssignActivity(process, mySecondActivity, _solderingCell);
        RaiseActivityUpdated(mySecondActivity, ActivityProgress.Ready);

        await Task.Delay(200);

        RaiseActivityUpdated(mySecondActivity, ActivityProgress.Running);
        RaiseProcessUpdated(process, ProcessProgress.Running);
    }

    private async Task StartFirstActivityAsync(Process process, AssemblyActivity myFirstActivity)
    {
        // ----------- First activity
        AssignActivity(process, myFirstActivity, _assemblyCell);
        RaiseProcessUpdated(process, ProcessProgress.Ready);
        RaiseActivityUpdated(myFirstActivity, ActivityProgress.Ready);

        await Task.Delay(200);

        RaiseActivityUpdated(myFirstActivity, ActivityProgress.Running);
        RaiseProcessUpdated(process, ProcessProgress.Running);
    }

    private void RaiseActivityUpdated(Activity activity, ActivityProgress progress)
    {
        _processFacadeMock.Raise(pm => pm.ActivityUpdated += null, new ActivityUpdatedEventArgs(activity, progress));
    }

    private void RaiseProcessUpdated(Process process, ProcessProgress progress)
    {
        _processFacadeMock.Raise(pm => pm.ProcessUpdated += null, new ProcessUpdatedEventArgs(process, progress));
    }

    private Activity AssignActivity(Process process, Activity activity, ICell cell)
    {
        activity.Process = process;
        activity.Tracing.ResourceId = cell.Id;
        process.AddActivity(activity);
        // Assign resources AFTER creation
        _activityTargets[activity] = [cell];
        return activity;
    }
}
