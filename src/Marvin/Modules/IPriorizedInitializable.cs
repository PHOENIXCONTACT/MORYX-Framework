using System;

namespace Marvin.Modules
{
    /// <summary>
    /// Inizializable component wich can set a priority for the initialization process
    /// </summary>
    public interface IPrioritizedInitializable : IInitializable
    {
        /// <summary>
        /// Priority of this instance compared to instances of the same type
        /// </summary>
        Priority Priority { get; }
    }

    /// <summary>
    /// Priority for the <see cref="IPrioritizedInitializable"/>. P0 is the highest priority.
    /// </summary>
    public struct Priority : IComparable<Priority>, IEquatable<Priority>
    {
        private readonly ushort _priority;

        /// <summary>
        /// Create new priority instance
        /// </summary>
        /// <param name="priority"></param>
        public Priority(ushort priority)
        {
            _priority = priority;
        }

        /// <summary>
        /// <value>0</value> means maximum priority
        /// </summary>
        public static Priority Max => P0;

        /// <summary>
        /// Minimum priority is the highest possible value
        /// </summary>
        public static Priority Min => new Priority(ushort.MaxValue);

        /// <summary>
        /// Priority value 0
        /// </summary>
        public static Priority P0 => new Priority(0);

        /// <summary>
        /// Priority value 1
        /// </summary>
        public static Priority P1 => new Priority(1);

        /// <summary>
        /// Priority value 2
        /// </summary>
        public static Priority P2 => new Priority(2);

        /// <summary>
        /// Priority value 3
        /// </summary>
        public static Priority P3 => new Priority(3);

        /// <summary>
        /// Priority value 4
        /// </summary>
        public static Priority P4 => new Priority(4);

        /// <summary>
        /// Custom value priority
        /// </summary>
        public static Priority P(ushort value) => new Priority(value);

        /// <inheritdoc />
        public int CompareTo(Priority other)
        {
            // Invert standard compare of ushort for reversed behavior of 
            // priorities
            return other._priority.CompareTo(_priority);
        }
        
        /// <inheritdoc />
        public bool Equals(Priority other)
        {
            return _priority == other._priority;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Priority && Equals((Priority) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _priority.GetHashCode();
        }

        /// <summary>
        /// Compare two priorities for equality
        /// </summary>
        public static bool operator ==(Priority a, Priority b)
        {
            return a._priority == b._priority;
        }

        /// <summary>
        /// Compare two priorities for inequality
        /// </summary>
        public static bool operator !=(Priority a, Priority b)
        {
            return a._priority == b._priority;
        }

        /// <summary>
        /// Compare two priorities
        /// </summary>
        public static bool operator >(Priority a, Priority b)
        {
            return a._priority < b._priority;
        }

        /// <summary>
        /// Compare two priorities
        /// </summary>
        public static bool operator <(Priority a, Priority b)
        {
            return a._priority > b._priority;
        }

        /// <summary>
        /// Compare two priorities
        /// </summary>
        public static bool operator >=(Priority a, Priority b)
        {
            return a._priority <= b._priority;
        }

        /// <summary>
        /// Compare two priorities
        /// </summary>
        public static bool operator <=(Priority a, Priority b)
        {
            return a._priority >= b._priority;
        }
    }
}