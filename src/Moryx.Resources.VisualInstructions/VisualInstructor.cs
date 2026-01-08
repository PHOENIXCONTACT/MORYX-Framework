// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.Resources.VisualInstructions.Properties;
using Moryx.Serialization;
using Moryx.Threading;
using Moryx.VisualInstructions;

namespace Moryx.Resources.VisualInstructions;

/// <summary>
/// Resource implementation for a visual instructor
/// </summary>
[ResourceRegistration]
public class VisualInstructor : Resource, IVisualInstructor, IVisualInstructionSource
{
    #region Dependencies

    /// <summary>
    /// Injected parallel operations for asynchronous execution
    /// </summary>
    public IParallelOperations ParallelOperations { get; set; }

    #endregion

    #region Fields and Properties

    private long _currentInstructionId;
    private readonly IDictionary<long, InstructionResultPair> _instructionDict = new Dictionary<long, InstructionResultPair>();
    private static readonly Action<ActiveInstructionResponse> _emptyInstructionResult = response => { };

    /// <summary>
    /// For editor: list of current instructions
    /// </summary>
    [EntrySerialize, ReadOnly(true)]
    [Display(Name = nameof(Strings.VisualInstructor_CurrentInstructions),
        Description = nameof(Strings.VisualInstructor_CurrentInstructions_Description), ResourceType = typeof(Strings))]
    public string CurrentInstructions
    {
        get
        {
            lock (_instructionDict)
                return string.Join(", ", _instructionDict.Keys);
        }
    }

    #endregion

    #region IVisualInstructionSource

    /// <inheritdoc />
    public string Identifier => Name;

    /// <inheritdoc />
    public IReadOnlyList<ActiveInstruction> Instructions
    {
        get
        {
            lock (_instructionDict)
                return _instructionDict.Values.Select(p => p?.Model).ToList();
        }
    }

    /// <inheritdoc />
    public void Completed(ActiveInstructionResponse response)
    {
        InstructionResultPair pair;

        Logger.Log(LogLevel.Debug, "Complete instruction with id {0} on {1}", response.Id, Name);

        lock (_instructionDict)
        {
            if (!_instructionDict.TryGetValue(response.Id, out pair))
                return;
        }

        if (pair.Model.Results.Count == 0)
            return;

        // Invoke callback
        pair.Callback(response);

        Clear(response.Id);
    }

    /// <inheritdoc />
    public event EventHandler<ActiveInstruction> Added;

    /// <inheritdoc />
    public event EventHandler<ActiveInstruction> Cleared;
    #endregion

    #region IVisualInstructor

    /// <inheritdoc />
    public long Display(ActiveInstruction activeInstruction)
    {
        var instructionId = ++_currentInstructionId;
        activeInstruction.Id = instructionId;

        var instructionPair = new InstructionResultPair
        {
            Model = activeInstruction,
            Callback = _emptyInstructionResult
        };

        lock (_instructionDict)
            _instructionDict.Add(activeInstruction.Id, instructionPair);

        Added?.Invoke(this, activeInstruction);

        return instructionId;
    }

    /// <inheritdoc />
    public void Display(ActiveInstruction instruction, int autoClearMs)
    {
        var instructionId = Display(instruction);
        ParallelOperations.ScheduleExecution(() => Clear(instructionId), autoClearMs, -1);
    }

    /// <inheritdoc />
    public long Execute(ActiveInstruction activeInstruction, Action<ActiveInstructionResponse> callback)
    {
        var instructionId = ++_currentInstructionId;
        activeInstruction.Id = instructionId;

        lock (_instructionDict)
            _instructionDict.Add(instructionId, new InstructionResultPair()
            {
                Model = activeInstruction,
                Callback = callback
            });

        Added?.Invoke(this, activeInstruction);

        return instructionId;
    }

    /// <inheritdoc />
    [EntrySerialize]
    [Display(Name = nameof(Strings.VisualInstructor_Clear), Description = nameof(Strings.VisualInstructor_Clear_Description), ResourceType = typeof(Strings))]
    public void Clear([Description("Id of the instruction")] long instructionId)
    {
        Logger.Log(LogLevel.Debug, "Clearing instruction with id {0} on {1}", instructionId, Name);

        if (!_instructionDict.ContainsKey(instructionId))
        {
            Logger.Log(LogLevel.Information, "Did not send clear via event for {0}, because the instruction is not in the instruction list", instructionId);
            return;
        }

        var instruction = _instructionDict[instructionId].Model;
        lock (_instructionDict)
        {
            if (!_instructionDict.Remove(instructionId))
                return;
        }

        Cleared?.Invoke(this, instruction);
    }
    #endregion

    private class InstructionResultPair
    {
        public ActiveInstruction Model { get; set; }

        public Action<ActiveInstructionResponse> Callback { get; set; }
    }
}