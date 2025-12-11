// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.AbstractionLayer.Activities;
using Moryx.ControlSystem.Cells;
using Moryx.VisualInstructions;

namespace Moryx.ControlSystem.VisualInstructions
{
    /// <summary>
    /// Extensions for <see cref="IVisualInstructor"/> and Sessions
    /// </summary>
    public static class VisualInstructorExtensions
    {
        /// <summary>
        /// Extensions for <see cref="IVisualInstructor"/>
        /// </summary>
        /// <param name="instructor"></param>
        extension(IVisualInstructor instructor)
        {
            /// <summary>
            /// Display the instructions on an activity
            /// </summary>
            public long Display(string title, ActivityStart activityStart)
            {
                var instructionParams = GetInstructionParameters(activityStart);
                return instructor.Display(new ActiveInstruction
                {
                    Title = title,
                    Instructions = instructionParams.Instructions,
                    Inputs = instructionParams.Inputs,
                });
            }

            /// <summary>
            /// Executes the instructions of an activity with defining own results
            /// </summary>
            public long Execute(string title, ActivityStart activityStart, IReadOnlyList<InstructionResult> results, Action<ActiveInstructionResponse> callback)
            {
                var instructionParams = GetInstructionParameters(activityStart);
                return instructor.Execute(new ActiveInstruction
                {
                    Title = title,
                    Instructions = instructionParams.Instructions,
                    Results = results,
                    Inputs = instructionParams.Inputs,
                }, callback);
            }

            /// <summary>
            /// Execute the instructions of an activity with type enum response
            /// </summary>
            public long Execute<TInput>(string title, ActivityStart activityStart, TInput input, Action<int, TInput, ActivityStart> callback)
                where TInput : class
            {
                var instructions = GetInstructionParameters(activityStart).Instructions;
                return Execute(instructor, title, activityStart, input, (result, populated, session) => callback(result, (TInput)populated, session), instructions);
            }

            /// <summary>
            /// Execute the instructions of an activity
            /// </summary>
            public long Execute(string title, ActivityStart activityStart, Action<int, ActivityStart> callback)
            {
                var instructions = GetInstructionParameters(activityStart);
                return Execute(instructor, title, activityStart, callback, instructions);
            }

            /// <summary>
            /// Executes an instruction based on a activity session (<see cref="ActivityStart"/>).
            /// Parameters can be set manually
            /// </summary>
            public long Execute(string title, ActivityStart activityStart, Action<int, ActivityStart> callback, VisualInstructionParameters parameters)
            {
                return Execute(instructor, title, activityStart, callback, parameters.Instructions);
            }

            /// <summary>
            /// Executes an instruction based on a activity session (<see cref="ActivityStart"/>).
            /// Parameters can be set manually
            /// </summary>
            public long Execute(string title, ActivityStart activityStart, Action<int, object, ActivityStart> callback, VisualInstructionParameters parameters)
            {
                return Execute(instructor, title, activityStart, parameters.Inputs, callback, parameters.Instructions);
            }

            /// <summary>
            /// Executes an instruction based on a activity session (<see cref="ActivityStart"/>).
            /// Parameters can be set manually
            /// </summary>
            public long Execute(string title, ActivityStart activityStart, Action<int, ActivityStart> callback, VisualInstruction[] parameters)
            {
                return Execute(instructor, title, activityStart, null, (result, input, activityStart) => callback(result, activityStart), parameters);
            }

            /// <summary>
            /// Executes an instruction based on a activity session (<see cref="ActivityStart"/>).
            /// Parameters can be set manually
            /// </summary>
            public long Execute(string title, ActivityStart activityStart, object inputs, Action<int, object, ActivityStart> callback, VisualInstruction[] parameters)
            {
                return ExecuteWithEnum(instructor, title, activityStart, inputs, callback, parameters);
            }

            /// <summary>
            /// Internal implementation of different overloads of 'Execute'
            /// </summary>
            private long ExecuteWithEnum(string title, ActivityStart activityStart, object inputs, Action<int, object, ActivityStart> callback, VisualInstruction[] parameters)
            {
                var activity = activityStart.Activity;
                var attr = activity.GetType().GetCustomAttribute<ActivityResultsAttribute>() ??
                           throw new ArgumentException($"Activity is not decorated with the {nameof(ActivityResultsAttribute)}");

                if (!attr.ResultEnum.IsEnum)
                {
                    throw new ArgumentException("Result type is not an enum!");
                }

                return instructor.Execute(new ActiveInstruction
                {
                    Title = title,
                    Instructions = parameters,
                    Results = EnumInstructionResult.PossibleResults(attr.ResultEnum),
                    Inputs = inputs
                }, instructionResponse => callback(EnumInstructionResult.ResultToEnumValue(attr.ResultEnum, instructionResponse.SelectedResult), instructionResponse.Inputs, activityStart));
            }
        }

        private static VisualInstructionParameters GetInstructionParameters(ActivityStart activity)
        {
            if (((IActivity<IParameters>)activity.Activity).Parameters is not VisualInstructionParameters parameters)
            {
                throw new ArgumentException($"Activity parameters are not of type {nameof(VisualInstructionParameters)}.");
            }

            return parameters;
        }
    }
}
