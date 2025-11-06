// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Processes;
using Moryx.ControlSystem.Cells;

namespace Moryx.ControlSystem.Processes
{
    /// <summary>
    /// Extension methods to handle sessions on process holders
    /// </summary>
    public static class ProcessHolderExtensions
    {
        #region Get Position
        /// <summary>
        /// Get the position by its <see cref="IProcessHolderPosition.Identifier"/>
        /// </summary>
        public static TPosition GetPositionByIdentifier<TPosition>(this IProcessHolderGroup<TPosition> group, string identifier)
            where TPosition : IProcessHolderPosition => GetPosition(group.Positions, t => t.Identifier == identifier);

        /// <summary>
        /// Get the position by its <see cref="IProcessHolderPosition.Identifier"/>
        /// </summary>
        public static IProcessHolderPosition GetPositionByIdentifier(this IProcessHolderGroup group, string identifier)
            => GetPosition(group.Positions, t => t.Identifier == identifier);

        /// <summary>
        /// Get the position by its <see cref="IProcessHolderPosition.Identifier"/>
        /// </summary>
        public static TPosition GetPositionByIdentifier<TPosition>(this IEnumerable<TPosition> positions, string identifier)
            where TPosition : IProcessHolderPosition => GetPosition(positions, t => t.Identifier == identifier);

        /// <summary>
        /// Get the position by its <see cref="IProcessHolderPosition.Session"/>
        /// </summary>
        public static TPosition GetPositionBySession<TPosition>(this IProcessHolderGroup<TPosition> group, Session session)
            where TPosition : IProcessHolderPosition => GetPosition(group.Positions, t => t.Session?.Id == session.Id);

        /// <summary>
        /// Get the position by its <see cref="IProcessHolderPosition.Session"/>
        /// </summary>
        public static IProcessHolderPosition GetPositionBySession(this IProcessHolderGroup group, Session session)
            => GetPosition(group.Positions, t => t.Session?.Id == session.Id);

        /// <summary>
        /// Get the position by its <see cref="IProcessHolderPosition.Session"/>
        /// </summary>
        public static TPosition GetPositionBySession<TPosition>(this IEnumerable<TPosition> positions, Session session)
            where TPosition : IProcessHolderPosition => GetPosition(positions, t => t.Session?.Id == session.Id);

        /// <summary>
        /// Get the position by its <see cref="IProcessHolderPosition.Process"/>
        /// </summary>
        public static TPosition GetPositionByProcessId<TPosition>(this IProcessHolderGroup<TPosition> group, long processId)
            where TPosition : IProcessHolderPosition => GetPosition(group.Positions, t => t.Process?.Id == processId);

        /// <summary>
        /// Get the position by its <see cref="IProcessHolderPosition.Process"/>
        /// </summary>
        public static IProcessHolderPosition GetPositionByProcessId(this IProcessHolderGroup group, long processId)
            => GetPosition(group.Positions, t => t.Process?.Id == processId);

        /// <summary>
        /// Get the position by its <see cref="IProcessHolderPosition.Process"/>
        /// </summary>
        public static TPosition GetPositionByProcessId<TPosition>(this IEnumerable<TPosition> positions, long processId)
            where TPosition : IProcessHolderPosition => GetPosition(positions, t => t.Process?.Id == processId);

        /// <summary>
        /// Get the position by id of the running activity
        /// </summary>
        public static TPosition GetPositionByActivityId<TPosition>(this IProcessHolderGroup<TPosition> group, long activityId)
            where TPosition : IProcessHolderPosition => GetPosition(group.Positions, t => (t.Session as ActivityStart)?.Activity.Id == activityId);

        /// <summary>
        /// Get the position by id of the running activity
        /// </summary>
        public static IProcessHolderPosition GetPositionByActivityId(this IProcessHolderGroup group, long activityId)
            => GetPosition(group.Positions, t => (t.Session as ActivityStart)?.Activity.Id == activityId);

        /// <summary>
        /// Get the position by id of the running activity
        /// </summary>
        public static TPosition GetPositionByActivityId<TPosition>(this IEnumerable<TPosition> positions, long activityId)
            where TPosition : IProcessHolderPosition => GetPosition(positions, t => (t.Session as ActivityStart)?.Activity.Id == activityId);

        /// <summary>
        /// Tries to get an empty <see cref="IProcessHolderPosition"/> from the <paramref name="group"/>
        /// </summary>
        /// <param name="group">The group to search</param>
        /// <param name="position">When this method returns, contains the first empty 
        /// <see cref="IProcessHolderPosition"/>, if any is found; otherwise <c>null</c></param>
        /// <returns>
        /// <c>true</c> if an empty <see cref="IProcessHolderPosition"/> was found; otherwise, <c>false</c>.
        /// </returns>
        public static bool TryGetEmptyPosition(this IProcessHolderGroup group, out IProcessHolderPosition position)
        {
            foreach (var possible in group.Positions)
            {
                if (!possible.IsEmpty())
                    continue;
                position = possible;
                return true;
            }
            position = default;
            return false;
        }

        private static TPosition GetPosition<TPosition>(IEnumerable<TPosition> positions, Func<TPosition, bool> filter)
            where TPosition : IProcessHolderPosition => positions.SingleOrDefault(filter);

        #endregion

        #region Get or Create Sessions

        /// <summary>
        /// Try cast and return the holders session property
        /// </summary>
        public static TSession ConvertSession<TSession>(this IProcessHolderPosition holderPosition)
            where TSession : Session => holderPosition.Session as TSession;

        /// <summary>
        /// Get or create a session for the <paramref name="position"/>, if it holds a process. Otherwise returns an empty enumerable.
        /// This is usually used when attaching to the control system.
        /// </summary>
        public static IEnumerable<Session> Attach(this ProcessHolderPosition position)
        {
            if (position.Session != null)
                return [position.Session];

            return position.Process == null ? [] : [position.StartSession()];
        }

        /// <summary>
        /// Get or create sessions for all <paramref name="positions"/> that have a process. 
        /// This is usually used when attaching to the control system.
        /// </summary>
        public static IEnumerable<Session> Attach(this IEnumerable<ProcessHolderPosition> positions)
            => positions.SelectMany(p => p.Attach());

        /// <summary>
        /// Get or create sessions for all <see cref="IProcessHolderGroup.Positions"/> that have a process. 
        /// This is usually used when attaching to the control system.
        /// </summary>
        public static IEnumerable<Session> Attach(this IProcessHolderGroup group)
        {
            var sessions = new List<Session>();
            foreach (var position in group.Positions)
            {
                if (position.Session is not null)
                {
                    sessions.Add(position.Session);
                    continue;
                }

                if (position.Process is null)
                    continue;

                if (position is ProcessHolderPosition php)
                    sessions.Add(php.StartSession());
            }
            return sessions;
        }

        /// <summary>
        /// Get a session if <paramref name="position"/> has one. Otherwise returns an empty enumerable. 
        /// This is usually used when detaching from the control system.
        /// </summary>
        public static IEnumerable<Session> Detach(this ProcessHolderPosition position)
            => position.Session != null ? [position.Session] : [];

        /// <summary>
        /// Gets the session from each <see cref="ProcessHolderPosition"/> in the <paramref name="positions"/> 
        /// that holds one. This is usually used when detaching from the control system.
        /// </summary>
        public static IEnumerable<Session> Detach(this IEnumerable<ProcessHolderPosition> positions)
            => positions.Where(p => p.Session != null).Select(p => p.Session);

        /// <summary>
        /// Gets the session from each <see cref="IProcessHolderPosition"/> in the <paramref name="group"/> 
        /// that holds one. This is usually used when detaching from the control system.
        /// </summary>
        public static IEnumerable<Session> Detach(this IProcessHolderGroup group)
            => group.Positions.Where(p => p.Session != null).Select(p => p.Session);

        #endregion

        #region Mounting

        /// <summary>
        /// Assign a <paramref name="process"/> to this position
        /// </summary>
        public static void Mount(this IProcessHolderPosition position, IProcess process)
            => position.Mount(new MountInformation(process, null));

        /// <summary>
        /// Assign a <paramref name="session"/> to this position
        /// </summary>
        public static void Mount(this IProcessHolderPosition position, Session session)
            => position.Mount(new MountInformation(null, session));

        /// <summary>
        /// Assign <paramref name="process"/> and <paramref name="session"/> to this position
        /// </summary>
        public static void Mount(this IProcessHolderPosition position, IProcess process, Session session)
            => position.Mount(new MountInformation(process, session));

        #endregion

        #region Status Checks

        /// <summary>
        /// Checks if the <paramref name="group"/> has no empty <see cref="IProcessHolderPosition"/>
        /// </summary>
        public static bool IsFull(this IProcessHolderGroup group)
            => group.Positions.All(position => !position.IsEmpty());

        /// <summary>
        /// Checks if the <paramref name="group"/> has no filled <see cref="IProcessHolderPosition"/>
        /// </summary>
        public static bool IsEmpty(this IProcessHolderGroup group)
            => group.Positions.All(position => position.IsEmpty());

        /// <summary>
        /// True if the <paramref name="position"/> has neither a <see cref="IProcessHolderPosition.Process"/> nor a 
        /// <see cref="IProcessHolderPosition.Session"/>; false otherwise.
        /// </summary>
        public static bool IsEmpty(this IProcessHolderPosition position)
            => position.Process is null && position.Session is null;

        /// <summary>
        /// Checks if the group holds a process with a finished activity having the matching result
        /// </summary>
        public static bool HasFinishedActivity<TPosition>(this IProcessHolderGroup<TPosition> group, long activityId, long activityResult)
            where TPosition : IProcessHolderPosition => group.Positions.Any(position => position.HasFinishedActivity(activityId, activityResult));

        /// <summary>
        /// Checks if the position holds a process with a finished activity having the matching result.
        /// </summary>
        public static bool HasFinishedActivity(this IProcessHolderPosition holderPosition, long activityId, long activityResult)
            => holderPosition.Process?.GetActivities(activity => activity.Id == activityId && activity.Result?.Numeric == activityResult).Any() == true;

        #endregion

        /// <summary>
        /// Access tracing of the current activity
        /// </summary>
        public static TTracing Tracing<TTracing>(this IProcessHolderPosition position)
            where TTracing : Tracing, new() => (position.Session as ActivityStart)?.Activity?.TransformTracing<TTracing>();

        /// <summary>
        /// Updates <see cref="IProcessHolderPosition.Session"/> and <see cref="IProcessHolderPosition.Process"/> by resetting the 
        /// <paramref name="position"/> and remounting the <paramref name="session"/>. For <see cref="ProcessHolderPosition"/>s 
        /// a direct update of the session and process is done on the object.
        /// </summary>
        /// <param name="position">The position to update</param>
        /// <param name="session">The updated session on the position</param>
        /// <exception cref="InvalidOperationException">Thrown if the given <paramref name="session"/> does not match 
        /// the session on the <paramref name="position"/></exception>
        public static void Update(this IProcessHolderPosition position, Session session)
        {
            if (position.Session.Id != session.Id)
                throw new InvalidOperationException($"Tried to update the {nameof(Session)} on an " +
                    $"{nameof(IProcessHolderPosition)} with a different session. Make sure to " +
                    $"{nameof(IProcessHolderPosition.Reset)} the {nameof(IProcessHolderPosition)} before " +
                    $"assigning a new {nameof(Session)}");

            if (position is ProcessHolderPosition explicitPosition)
            {
                explicitPosition.Session = session;
                explicitPosition.AssignProcess(session.Process);
                return;
            }

            position.Reset();
            position.Mount(session.Process, session);
        }
    }
}
