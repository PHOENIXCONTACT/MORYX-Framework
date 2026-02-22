// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.ControlSystem.Activities;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.Processes;
using Moryx.ControlSystem.Simulation;
using Moryx.Logging;
using Moryx.Simulation.Simulator.Extensions;
using Moryx.Threading;
using Moryx.Tools;
using System.Collections.Concurrent;

namespace Moryx.ControlSystem.Simulator.Implementation;

[Component(LifeCycle.Singleton, typeof(IProcessSimulator))]
internal sealed class ProcessSimulator : IProcessSimulator
{
    private readonly Random _simulationRandomness = new();
    private readonly List<ISimulationDriver> _drivers = [];
    private readonly ConcurrentDictionary<long, SemaphoreSlim> _driverSemaphores = new();
    // ToDo: ProcessMovement is very inefficient to hash, look for better options
    // Process.Id is invalid, as in parallel worplans two movements for one process can be triggered
    private readonly ConcurrentDictionary<ProcessMovement, int> _scheduledMovements = [];
    private readonly ConcurrentDictionary<long, int> _scheduledActivitis = [];

    private bool _running;

    #region Dependencies

    public ModuleConfig Config { get; set; }

    public IParallelOperations ParallelOperations { get; set; }

    public IProcessControl ProcessControl { get; set; }

    public IResourceManagement ResourceManagement { get; set; }

    [UseChild(nameof(ProcessSimulator))]
    public IModuleLogger Logger { get; set; }

    #endregion


    public void Start()
    {
        _running = true;
        // ToDo: Replace with safe/proxified resources. Currently not possible
        // as drivers are usually generic and generics cannot be proxified.
        ResourceManagement.GetResourcesUnsafe<ISimulationDriver>(x => true)
            .ForEach(RegisterDriver);

        ResourceManagement.ResourceAdded += OnResourceAdded;
        ResourceManagement.ResourceRemoved += OnResourceRemoved;

        // Sync any activities we might have missed
        var activities = ProcessControl.GetRunningProcesses()
            .SelectMany(p => p.GetActivities(a => a.Result == null))
            .ToList();

        ProcessControl.ActivityUpdated += OnActivityUpdated;

        // Process missed activities
        foreach (var activity in activities)
        {
            if (activity.Tracing.Started == null)
                ActivityReady(activity);
            else
                ActivityRunning(activity);
        }
    }

    private void SimulationStateChanged(object sender, SimulationState newState)
    {
        var driver = (ISimulationDriver)sender;
        var cell = driver.Usages.FirstOrDefault();
        var runningProcesses = ProcessControl.GetRunningProcesses().ToList();

        if (newState == SimulationState.Offline || newState > SimulationState.Idle)
            return;

        Activity nextActivity;
        lock (_scheduledMovements)
        {
            nextActivity = runningProcesses
                .Where(p => _scheduledMovements.Keys.All(m => m.Process.Id != p.Id)) // No processes in transit                                 
                .Select(p => new
                {
                    Activity = p.NextActivity(),
                    Targets = ProcessControl.Targets(p).ToList()
                })
                .Where(pat => pat.Targets.Any(t => t.Id == cell.Id))
                .Select(pat => new
                {
                    pat.Activity,
                    Index = pat.Targets.IndexOf(pat.Targets.First(t => t.Id == cell.Id))
                })
                .OrderBy(pair => pair.Index)
                .FirstOrDefault(pair => pair.Index >= 0 && pair.Activity != null)?.Activity;
        }

        if (nextActivity is null) return;

        SimulateReady(driver, nextActivity);
    }

    private void OnResourceAdded(object sender, IResource e)
    {
        if (e is ISimulationDriver driver)
        {
            RegisterDriver(driver);
        }
    }

    private void RegisterDriver(ISimulationDriver driver)
    {
        _drivers.Add(driver);
        // Note: We intentionally keep the semaphore in the dictionary to avoid races
        // that could occur when removing/disposing it while another thread tries to use it.
        _driverSemaphores.TryAdd(driver.Id, new SemaphoreSlim(1, 1));
        driver.SimulatedStateChanged += SimulationStateChanged;
    }

    private void OnResourceRemoved(object sender, IResource e)
    {
        if (e is ISimulationDriver driver)
        {
            UnregisterDriver(driver);
            _drivers.Remove(driver);
        }
    }

    private void UnregisterDriver(ISimulationDriver driver)
    {
        // Dropping scheduled movements is not necessary here,
        // as each movement needs to be checked for rerouting before completion anyway
        _driverSemaphores.Remove(driver.Id, out var semaphore);
        semaphore.Dispose();
        driver.SimulatedStateChanged -= SimulationStateChanged;
    }

    public void Stop()
    {
        _running = false;
        StopScheduledMovements();

        FinishSimulatedActivities();

        UnregisterDependencyEvents();

        _drivers.ForEach(UnregisterDriver);
        _drivers.Clear();
    }

    private void StopScheduledMovements()
    {
        Parallel.ForEach(_scheduledMovements, kvp => ParallelOperations.StopExecution(kvp.Value));
        _scheduledMovements.Clear();
    }

    private void FinishSimulatedActivities()
    {
        var expectedMaxExecutionTime = TimeSpan.FromMilliseconds(Math.Max(Config.DefaultExecutionTime,
            Config.SpecificExecutionTimeSettings.MaxBy(s => s.ExecutionTime)?.ExecutionTime ?? 0) / Config.Acceleration + 1);

        if (!_scheduledActivitis.WaitUntilEmpty(expectedMaxExecutionTime, expectedMaxExecutionTime / 100))
        {
            Logger.LogWarning("Stopped {name} before all simulated activities returned.", nameof(ProcessSimulator));
        }
    }

    private void UnregisterDependencyEvents()
    {
        ResourceManagement.ResourceAdded -= OnResourceAdded;
        ResourceManagement.ResourceRemoved -= OnResourceRemoved;
        ProcessControl.ActivityUpdated -= OnActivityUpdated;
    }

    private void OnActivityUpdated(object sender, ActivityUpdatedEventArgs e)
    {
        var activity = e.Activity;
        var isSetupActivity = activity is IControlSystemActivity ca && ca.Classification != ActivityClassification.Production;

        switch (e.Progress)
        {
            case ActivityProgress.Ready when !isSetupActivity:
                ActivityReady(activity);
                break;
            case ActivityProgress.Running:
                ActivityRunning(activity);
                break;
        }
    }

    /// <summary>
    /// Production Activity is ready and transport initiated
    /// </summary>
    /// <param name="activity"></param>
    private void ActivityReady(Activity activity)
    {
        var process = activity.Process;

        var cells = ProcessControl.Targets(activity);
        var drivers = GetDriversByPriority(cells.ToList());
        if (drivers.Count == 0)
        {
            Logger.LogDebug("Skipping 'Ready' and movement for activity '{activity}': Could not find matching {name}.", activity, nameof(ISimulationDriver));
            return;
        }

        if (!_running)
        {
            Logger.LogDebug("Skipping 'Ready' and movement for activity '{activity}': {name} is stopped.", activity, nameof(ProcessSimulator));
            return;
        }

        var movementTarget = drivers.First();
        // Start directly if the process does neither have a current location, nor is on the move
        if (process.GetActivities().All(a => a.Result == null) || movementTarget.Usages.Any(c => c.Id == process.LastActivity()?.Tracing.ResourceId))
        {
            SimulateReady(movementTarget, activity);
            return;
        }

        SimulateMovement(activity, process, movementTarget);
    }

    private void SimulateReady(ISimulationDriver target, Activity activity)
    {
        // Ensures calls for the same driver are serialized while calls for different drivers can run in parallel.
        _driverSemaphores.TryGetValue(target.Id, out var sem);
        sem.Wait();

        try
        {
            if (target.SimulatedState > SimulationState.Idle)
            {
                Logger.LogDebug("Skipping 'Ready' on driver {id}-{name} for activity '{activity}': Awaiting driver state change from {current} to {target}",
                            target.Id, target.Name, activity, target.SimulatedState, nameof(SimulationState.Idle));
                return;
            }

            Logger.LogDebug("Simulating 'Ready' on driver {id}-{name} for activity '{activity}'.", target.Id, target.Name, activity);
            target.Ready(activity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not simulate 'Ready' on driver {id}-{name}: Unexpected exception.", target.Id, target.Name);
        }
        finally
        {
            sem.Release();
        }
    }

    private void SimulateMovement(Activity activity, Process process, ISimulationDriver movementTarget)
    {
        Logger.LogDebug("Simulating movement to driver {0}-{1} for activity '{2}'.",
                    movementTarget.Id, movementTarget.Name, activity.ToString());

        var duration = CalculateDuration();
        var movement = new ProcessMovement
        {
            Process = process,
            NextActivity = activity,
            Target = movementTarget,
            Started = DateTime.Now,
            Duration = new TimeSpan(0, 0, 0, 0, duration)
        };
        var schedule = ParallelOperations.ScheduleExecution(CompleteMovement, movement, duration, -1);
        _scheduledMovements.TryAdd(movement, schedule);
    }

    /// <summary>
    /// Activity has switched to running and we start the completion timer
    /// </summary>
    /// <param name="activity"></param>
    private void ActivityRunning(Activity activity)
    {
        // Check if this activity should be executed by an simulation driver
        var driver = _drivers.FirstOrDefault(d => d.Usages.Any(cell => cell.Id == activity.Tracing.ResourceId));
        if (driver == null)
        {
            Logger.LogDebug("Skipping simulation for activity '{0}': Could not find matching {1}.",
                activity.ToString(), nameof(ISimulationDriver));
            return;
        }

        var executionTime = CalculateExecutionTime(activity);
        executionTime = (int)(executionTime * (0.9 + _simulationRandomness.NextDouble() / 5));
        var completionArgs = new ActivityAndDriver(activity, driver);
        var schedule = ParallelOperations.ScheduleExecution(CompleteActivity, completionArgs, executionTime, -1);
        _scheduledActivitis.TryAdd(activity.Id, schedule);
    }

    private int CalculateExecutionTime(Activity activity)
    {
        if (activity.Parameters is ISimulationParameters simulationParameters)
            return (int)(simulationParameters.ExecutionTime.TotalMilliseconds / Config.Acceleration);

        var settings = Config.SpecificExecutionTimeSettings.Where(s => s.Activity.Equals(activity.GetType().FullName));
        if (!settings.Any())
        {
            return (int)(Config.DefaultExecutionTime / Config.Acceleration);
        }

        var resourceId = activity.Tracing.ResourceId;
        var specificSetting = settings.FirstOrDefault(s => s.CellId == resourceId);
        if (specificSetting != null)
            return (int)(specificSetting.ExecutionTime / Config.Acceleration);

        specificSetting = settings.FirstOrDefault(s => s.CellId == 0);
        if (specificSetting != null)
            return (int)(specificSetting.ExecutionTime / Config.Acceleration);

        return (int)(Config.DefaultExecutionTime / Config.Acceleration);
    }

    /// <summary>
    /// Reorder the list of drivers based on the provided list of cells
    /// </summary>
    /// <param name="cells">ordered list of available cells</param>
    /// <returns></returns>
    private List<ISimulationDriver> GetDriversByPriority(List<ICell> cells)
    {
        return cells.Select(c => _drivers.FirstOrDefault(d => d.Usages.Any(u => u.Id == c.Id)))
            .Where(c => c != null).ToList();
    }

    private int CalculateDuration()
    {
        return (int)Math.Ceiling(Convert.ToDouble(Config.MovingDuration) * 1 / Convert.ToDouble(Config.Acceleration));
    }

    private void CompleteMovement(ProcessMovement movement)
    {
        _scheduledMovements.Remove(movement, out _);

        if (movement.NextActivity.Tracing.Started is not null)
        {
            Logger.LogDebug("Skipping 'Ready' on driver {0}-{1} for activity '{2}': Activity has already started or was canceled.",
                    movement.Target.Id, movement.Target.Name, movement.NextActivity.ToString());
            return;
        }

        var cells = ProcessControl.Targets(movement.Process).ToList();
        var targets = GetDriversByPriority(cells);
        if (!targets.Contains(movement.Target))
        {
            Logger.LogDebug("Previous target driver {0}-{1} for activity '{2}' is unavailable. Start routing to next target.",
                    movement.Target.Id, movement.Target.Name, movement.NextActivity.ToString());
            SimulateMovement(movement.NextActivity, movement.Process, targets?.FirstOrDefault());
            return;
        }

        SimulateReady(movement.Target, movement.NextActivity);
    }

    private void CompleteActivity(ActivityAndDriver aad)
    {
        var driver = aad.Driver;
        var activity = aad.Activity;

        if (activity.Tracing.Completed is not null)
        {
            Logger.LogDebug("Skipping 'Result' on driver {0}-{1} for activity '{2}': Activity is already completed.",
                    driver.Id, driver.Name, activity.ToString());
            return;
        }

        var randResult = _simulationRandomness.Next(100);
        var result = randResult <= Config.SuccessRate ? 0 : 1;

        Logger.LogDebug("Simulating 'Result' on driver {0}-{1} for activity '{2}'.",
            driver.Id, driver.Name, activity.ToString());

        try
        {
            driver.Result(new SimulationResult { Result = result, Activity = activity });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not simulate 'Result' on driver {0}-{1}: Unexpected exception.", driver.Id, driver.Name);
        }

        _scheduledActivitis.TryRemove(activity.Id, out _);
    }

    private class ActivityAndDriver
    {
        public Activity Activity { get; }

        public ISimulationDriver Driver { get; }

        public ActivityAndDriver(Activity activity, ISimulationDriver driver)
        {
            Activity = activity;
            Driver = driver;
        }
    }
}
