// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.VisualInstructions
{
    /// <summary>
    /// Extensions for <see cref="IVisualInstructor"/>
    /// </summary>
    public static class VisualInstructorExtensions
    {
        /// <summary>
        /// Extensions for <see cref="IVisualInstructor"/>
        /// </summary>
        extension(IVisualInstructor instructor)
        {
            /// <summary>
            /// Only display these instructions
            /// Have to be cleared with the <see cref="IVisualInstructor.Clear"/> method
            /// </summary>
            public long Display(string title, VisualInstructionParameters parameter)
            {
                return instructor.Display(new ActiveInstruction
                {
                    Title = title,
                    Instructions = parameter.Instructions,
                    Inputs = parameter.Inputs,
                });
            }

            /// <summary>
            /// Only display these instructions
            /// Instruction will automatically cleared after the given time
            /// </summary>
            public void Display(string title, VisualInstructionParameters parameter, int autoClearMs)
            {
                instructor.Display(new ActiveInstruction
                {
                    Title = title,
                    Instructions = parameter.Instructions,
                    Inputs = parameter.Inputs,
                }, autoClearMs);
            }

            /// <summary>
            /// Show a visual instruction text message
            /// </summary>
            /// <param name="sender">The sender of the message</param>
            /// <param name="message">The message to be displayed</param>
            /// <returns>The id of the instruction</returns>
            public long DisplayMessage(string sender, string message)
            {
                return instructor.DisplayMessages(sender, [message]);
            }

            /// <summary>
            /// Show a set of messages as a visual instruction
            /// </summary>
            /// <param name="sender">The sender of the message</param>
            /// <param name="messages">The messages to be displayed</param>
            /// <returns>The id of the instruction</returns>
            public long DisplayMessages(string sender, string[] messages)
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
            /// <param name="sender">The sender of the message</param>
            /// <param name="message">The message to be displayed</param>
            /// <param name="autoClearMs">Time after which the message will be cleared</param>
            public void DisplayMessage(string sender, string message, int autoClearMs)
            {
                instructor.DisplayMessages(sender, [message], autoClearMs);
            }

            /// <summary>
            /// Show a set of messages as a visual instruction
            /// </summary>
            /// <param name="sender">The sender of the message</param>
            /// <param name="messages">The messages to be displayed</param>
            /// <param name="autoClearMs">Time after which the messages will be cleared</param>
            public void DisplayMessages(string sender, string[] messages, int autoClearMs)
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
            public long Execute(string title, VisualInstructionParameters parameter, IReadOnlyList<InstructionResult> results, Action<ActiveInstructionResponse> callback)
            {
                return instructor.Execute(new ActiveInstruction
                {
                    Title = title,
                    Instructions = parameter.Instructions,
                    Results = results,
                    Inputs = parameter.Inputs,
                }, callback);
            }

            /// <summary>
            /// Execute these instructions and report the result on completion
            /// Can (but must not) be cleared with the <see cref="IVisualInstructor.Clear"/> method
            /// </summary>
            /// <typeparam name="T">Type of enum used for possible instruction results</typeparam>
            public long Execute<T>(string title, VisualInstructionParameters parameter, Action<T> callback) where T : Enum
            {
                return instructor.Execute(
                    title,
                    parameter,
                    EnumInstructionResult.PossibleResults(typeof(T)),
                    result => callback(EnumInstructionResult.ResultToGenericEnumValue<T>(result.SelectedResult)));
            }
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
