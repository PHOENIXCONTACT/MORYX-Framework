// Copyright (c) 2024, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moryx.AbstractionLayer;
using Moryx.ControlSystem.Cells;

namespace Moryx.ControlSystem.VisualInstructions
{
    /// <summary>
    /// Extensions for <see cref="IVisualInstructor"/>
    /// </summary>
    public static class VisualInstructorExtensions
    {
        /// <summary>
        /// Only display these instructions
        /// Have to be cleared with the <see cref="IVisualInstructor.Clear"/> method
        /// </summary>
        public static long Display(this IVisualInstructor instructor, string title, IVisualInstructions parameter)
        {
            return instructor.Display(new ActiveInstruction
            {
                Title = title,
                Instructions = parameter.Instructions
            });
        }

        /// <summary>
        /// Only display these instructions
        /// Instruction will automatically cleared after the given time
        /// </summary>
        public static void Display(this IVisualInstructor instructor, string title, IVisualInstructions parameter, int autoClearMs)
        {
            instructor.Display(new ActiveInstruction
            {
                Title = title,
                Instructions = parameter.Instructions
            }, autoClearMs);
        }

        /// <summary>
        /// Display the instructions on an activity
        /// </summary>
        public static long Display(this IVisualInstructor instructor, string title, ActivityStart activityStart)
        {
            var instructions = GetInstructions(activityStart);
            return instructor.Display(new ActiveInstruction
            {
                Title = title,
                Instructions = instructions
            });
        }

        /// <summary>
        /// Show a visual instruction text message
        /// </summary>
        /// <param name="instructor">The instructor to display the message</param>
        /// <param name="sender">The sender of the message</param>
        /// <param name="message">The message to be displayed</param>
        /// <returns>The id of the instruction</returns>
        public static long DisplayMessage(this IVisualInstructor instructor, string sender, string message)
        {
            return instructor.DisplayMessages(sender, [message]);
        }

        /// <summary>
        /// Show a set of messages as a visual instruction 
        /// </summary>
        /// <param name="instructor">The instructor to display the messages</param>
        /// <param name="sender">The sender of the message</param>
        /// <param name="messages">The messages to be displayed</param>
        /// <returns>The id of the instruction</returns>
        public static long DisplayMessages(this IVisualInstructor instructor, string sender, string[] messages)
        {
            var instructions = messages.Select(AsInstruction).ToArray();
            return instructor.Display(new ActiveInstruction
            {
                Title = sender,
                Instructions = instructions
            });
        }

        /// <summary>
        /// Show a visual instruction text message
        /// </summary>
        /// <param name="instructor">The instructor to display the message</param>
        /// <param name="sender">The sender of the message</param>
        /// <param name="message">The message to be displayed</param>
        /// <param name="autoClearMs">Time after which the message will be cleared</param>
        public static void DisplayMessage(this IVisualInstructor instructor, string sender, string message, int autoClearMs)
        {
            instructor.DisplayMessages(sender, [message], autoClearMs);
        }

        /// <summary>
        /// Show a set of messages as a visual instruction 
        /// </summary>
        /// <param name="instructor">The instructor to display the messages</param>
        /// <param name="sender">The sender of the message</param>
        /// <param name="messages">The messages to be displayed</param>
        /// <param name="autoClearMs">Time after which the messages will be cleared</param>
        public static void DisplayMessages(this IVisualInstructor instructor, string sender, string[] messages, int autoClearMs)
        {
            var instructions = messages.Select(AsInstruction).ToArray();
            instructor.Display(new ActiveInstruction
            {
                Title = sender,
                Instructions = instructions
            }, autoClearMs);
        }

        /// <summary>
        /// Execute these instructions based on the given activity and report the result on completion
        /// Can (but must not) be cleared with the <see cref="IVisualInstructor.Clear"/> method
        /// </summary>
        public static long Execute(this IVisualInstructor instructor, string title, IVisualInstructions parameter, IReadOnlyList<InstructionResult> results, Action<ActiveInstructionResponse> callback)
        {
            return instructor.Execute(new ActiveInstruction
            {
                Title = title,
                Instructions = parameter.Instructions,
                Results = results
            }, callback);
        }

        /// <summary>
        /// Execute these instructions and report the result on completion
        /// Can (but must not) be cleared with the <see cref="IVisualInstructor.Clear"/> method
        /// </summary>
        /// <typeparam name="T">Type of enum used for possible instruction results</typeparam>
        public static long Execute<T>(this IVisualInstructor instructor, string title, IVisualInstructions parameter, Action<T> callback) where T : Enum
        {
            return instructor.Execute(
                title,
                parameter,
                EnumInstructionResult.PossibleResults(typeof(T)),
                result => callback(EnumInstructionResult.ResultToGenericEnumValue<T>(result.SelectedResult)));
        }

        /// <summary>
        /// Executes the instructions of an activity with defining own results
        /// </summary>
        public static long Execute(this IVisualInstructor instructor, string title, ActivityStart activityStart, IReadOnlyList<InstructionResult> results, Action<ActiveInstructionResponse> callback)
        {
            var instructions = GetInstructions(activityStart);
            return instructor.Execute(new ActiveInstruction
            {
                Title = title,
                Instructions = instructions,
                Results = results
            }, callback);
        }

        /// <summary>
        /// Execute the instructions of an activity with type enum response
        /// </summary>
        public static long Execute<TInput>(this IVisualInstructor instructor, string title, ActivityStart activityStart, TInput input, Action<int, TInput, ActivityStart> callback)
            where TInput : class
        {
            var instructions = GetInstructions(activityStart);
            return Execute(instructor, title, activityStart, input, (result, populated, session) => callback(result, (TInput)populated, session), instructions);
        }

        /// <summary>
        /// Execute the instructions of an activity
        /// </summary>
        public static long Execute(this IVisualInstructor instructor, string title, ActivityStart activityStart, Action<int, ActivityStart> callback)
        {
            var instructions = GetInstructions(activityStart);
            return Execute(instructor, title, activityStart, callback, instructions);
        }

        /// <summary>
        /// Executes an instruction based on a activity session (<see cref="ActivityStart"/>).
        /// Parameters can be set manually
        /// </summary>
        public static long Execute(this IVisualInstructor instructor, string title, ActivityStart activityStart, Action<int, ActivityStart> callback, IVisualInstructions parameters)
        {
            return Execute(instructor, title, activityStart, callback, parameters.Instructions);
        }

        /// <summary>
        /// Executes an instruction based on a activity session (<see cref="ActivityStart"/>).
        /// Parameters can be set manually
        /// </summary>
        public static long Execute(this IVisualInstructor instructor, string title, ActivityStart activityStart, Action<int, ActivityStart> callback, VisualInstruction[] parameters)
        {
            return Execute(instructor, title, activityStart, null, (result, input, activityStart) => callback(result, activityStart), parameters);
        }

        /// <summary>
        /// Executes an instruction based on a activity session (<see cref="ActivityStart"/>).
        /// Parameters can be set manually
        /// </summary>
        public static long Execute(this IVisualInstructor instructor, string title, ActivityStart activityStart, object inputs, Action<int, object, ActivityStart> callback, VisualInstruction[] parameters)
        {
            return ExecuteWithEnum(instructor, title, activityStart, inputs, callback, parameters);
        }

        /// <summary>
        /// Internal implementation of different overloads of 'Execute'
        /// </summary>
        private static long ExecuteWithEnum(this IVisualInstructor instructor, string title, ActivityStart activityStart, object inputs, Action<int, object, ActivityStart> callback, VisualInstruction[] parameters)
        {
            var activity = activityStart.Activity;

            var attr = activity.GetType().GetCustomAttribute<ActivityResultsAttribute>();
            if (attr == null)
                throw new ArgumentException($"Activity is not decorated with the {nameof(ActivityResultsAttribute)}");

            if (!attr.ResultEnum.IsEnum)
                throw new ArgumentException("Result type is not an enum!");

            var results = EnumInstructionResult.PossibleResults(attr.ResultEnum);
            var resultObjects = EnumInstructionResult.PossibleInstructionResults(attr.ResultEnum);

            return instructor.Execute(new ActiveInstruction
            {
                Title = title,
                Instructions = parameters,
                Results = results,
                Inputs = inputs
            }, instructionResponse => callback(EnumInstructionResult.ResultToEnumValue(attr.ResultEnum, instructionResponse.SelectedResult), instructionResponse.Inputs, activityStart));
        }

        private static VisualInstruction[] GetInstructions(ActivityStart activity)
        {
            var parameters = ((IActivity<IParameters>)activity.Activity).Parameters as IVisualInstructions;
            if (parameters == null)
                throw new ArgumentException($"Activity parameters are not of type {nameof(IVisualInstructions)}.");

            return parameters.Instructions;
        }

        /// <summary>
        /// Returns a text instruction for the given string.
        /// </summary>
        /// <param name="text">Instruction text</param>
        /// <returns><see cref="VisualInstruction"/> with type `Text` the given string as content</returns>
        public static VisualInstruction AsInstruction(this string text)
        {
            return new VisualInstruction
            {
                Content = text,
                Type = InstructionContentType.Text,
            };
        }
    }
}
