// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Workplans.Editing.Components
{
    internal interface IWorkplanEditor: IPlugin
    {
        /// <summary>
        /// Available steps
        /// </summary>
        IReadOnlyList<Type> AvailableSteps { get; }

        /// <summary>
        /// Open session to edit a workplan
        /// </summary>
        IWorkplanEditingSession EditWorkplan(Workplan workplan, bool duplicate);

        /// <summary>
        /// Access session
        /// </summary>
        IWorkplanEditingSession this[string token] { get; }

        /// <summary>
        /// Close the session
        /// </summary>
        void CloseSession(string token);
    }
}
