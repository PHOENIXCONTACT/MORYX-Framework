// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Activities;
using Moryx.ControlSystem.Cells;

namespace Moryx.ControlSystem.ProcessEngine.Processes
{
    /// <summary>
    /// Session that was reported during <see cref="IControlSystemBound.ControlSystemAttached"/>
    /// or <see cref="IControlSystemBound.ControlSystemDetached"/>
    /// </summary>
    internal class ResourceAndSession
    {
        public ResourceAndSession(ICell resource, Session session)
        {
            Resource = resource;
            Session = session;
        }

        public ICell Resource { get; }

        public Session Session { get; }

        public ReadyToWork ReadyToWork => Session as ReadyToWork;

        /// <summary>
        /// Check if this instance matches a given <see cref="ActivityData"/>
        /// </summary>
        public bool MatchesActivity(ActivityData activityData)
        {
            // Dispatch only configured or running activities
            if (activityData.State < ActivityState.Configured || activityData.State > ActivityState.Running)
                return false;

            // Only redeliver running activities if they refer to a process
            if (activityData.State >= ActivityState.Running && Session.Reference.IsEmpty)
                return false;

            // Do not dispatch activities for incomplete or stopping processes
            if (activityData.ProcessData.State < ProcessState.EngineStarted || activityData.ProcessData.State >= ProcessState.Stopping)
                return false;

            // Make sure activity can run in this resource
            if (activityData.Targets.All(possible => possible != Resource))
                return false;

            // Check if session accepts the classification
            var activity = activityData.Activity;
            if ((activityData.Classification & Session.AcceptedClassification) == 0)
                return false;

            // If a reference is given, it must match the activities process
            if (Session.Reference.HasReference && !Session.Reference.Matches(activity.Process))
                return false;

            if (Session.Reference.IsEmpty && activity.ProcessRequirement == ProcessRequirement.Required)
                return false;

            // Validate the constrains on the rtw with the process.
            if (ReadyToWork?.Constraints.Any(c => !c.Check(activity.Process)) == true)
                return false;

            // If we made it here, all conditions are met
            return true;
        }

        /// <summary>
        /// Check if this instance matches another <see cref="ResourceAndSession"/>
        /// </summary>
        public bool Equals(ResourceAndSession other)
        {
            return Resource == other.Resource
                   && Session.GetType() == other.Session.GetType()
                   && Session.AcceptedClassification == other.Session.AcceptedClassification
                   && Session.Reference == other.Session.Reference;
        }
    }
}
