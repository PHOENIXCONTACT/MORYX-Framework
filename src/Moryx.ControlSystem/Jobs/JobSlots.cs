// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;

namespace Moryx.ControlSystem.Jobs
{
    /// <summary>
    /// Wrapper object for the <see cref="JobSlots{TSlot}"/> helper class
    /// </summary>
    public class JobSlotWrapper
    {
        /// <summary>
        /// Job data object wrapped by this instance
        /// </summary>
        public Job Target { get; protected set; }

        /// <summary>
        /// Check if the job can be assigned to this slot
        /// </summary>
        public virtual bool CanAssign(Job target) => true;

        /// <summary>
        /// Initialize the slot wrapper after it was created AND assigned for the first time
        /// </summary>
        public virtual void Assign(Job target)
        {
            Target = target;
        }
    }

    /// <summary>
    /// Use <see cref="JobSlots{TSlot}"/> with the empty default wrapper
    /// </summary>
    public class JobSlots : JobSlots<JobSlotWrapper>
    {
        /// <summary>
        /// Create a sized <see cref="JobSlots"/> instance
        /// </summary>
        public JobSlots(int slotCount) : base(slotCount)
        {
        }
    }

    /// <summary>
    /// Class that wraps slot handling of a scheduler
    /// </summary>
    public class JobSlots<TSlot> : IEnumerable<TSlot>
        where TSlot : JobSlotWrapper, new()
    {
        /// <summary>
        /// All slots managed by this instance
        /// </summary>
        private readonly List<TSlot> _slots = new List<TSlot>();

        /// <summary>
        /// Create a new job slot wrapper class
        /// </summary>
        public JobSlots(int slotCount)
        {
            AvailableSlots = slotCount;
            for (int i = 0; i < slotCount; i++)
                _slots.Add(new TSlot()); // Prepare slot wrapper for each slot
        }

        /// <summary>
        /// Size of the slots wrapper
        /// </summary>
        public int Size => _slots.Count;

        /// <summary>
        /// Determine the number of empty slots
        /// </summary>
        /// <returns></returns>
        public int AvailableSlots { get; private set; }

        /// <summary>
        /// Direct access to the underlying array
        /// </summary>
        public TSlot this[int index] => _slots[index];

        /// <summary>
        /// Retrieve the first empty slot
        /// </summary>
        /// <returns></returns>
        public TSlot Empty() => Wrapper(null);

        /// <summary>
        /// Retrieve the wrapper object for a given job
        /// </summary>
        public TSlot Wrapper(Job job) => _slots.FirstOrDefault(s => s?.Target == job);

        /// <summary>
        /// Check if a given job occupies a slot
        /// </summary>
        public bool HasSlot(Job job) => _slots.Any(s => s.Target == job);

        /// <summary>
        /// Change the size of the <see cref="JobSlots{TSlot}"/> instance
        /// </summary>
        public bool TryResize(int size)
        {
            // If the size is identical this is easy
            if (size == Size)
                return true;

            // Make sure the new size fits all assigned slots
            // Decreasing is only possible if all taken slots fit in the new size
            if (size < Size - AvailableSlots)
                return false;

            // Now add or remove slots until the size matches
            AvailableSlots += size - Size;
            while (size != Size)
            {
                if (size > Size)
                    _slots.Add(new TSlot());
                else
                    _slots.RemoveAll(s => s.Target == null);
            }
            return true;
        }

        /// <summary>
        /// Assign a job to an empty slot
        /// </summary>
        public bool TryAssign(Job runningJob)
        {
            var result = TryReplace(null, runningJob);
            if (result)
                AvailableSlots--;
            return result;
        }

        /// <summary>
        /// Release the slot occupied by a job
        /// </summary>
        public bool TryRelease(Job runningJob)
        {
            var result = TryReplace(runningJob, null);
            if (result)
                AvailableSlots++;
            return result;
        }

        /// <summary>
        /// Replace the slot occupied by one job with another one
        /// </summary>
        public bool TryReplace(Job currentReference, Job newReference)
        {
            // Get the wrapper of the current reference
            var currentSlot = _slots.FirstOrDefault(s => s.Target == currentReference && s.CanAssign(newReference));
            // We failed to assign this job to a slot
            if (currentSlot == null)
                return false;

            // Assign new job to the slot
            currentSlot.Assign(newReference);
            return true;
        }

        /// <summary>
        /// Get enumerator to iterate over the jobs
        /// </summary>
        public IEnumerator<TSlot> GetEnumerator()
        {
            // Use implicit cast to access enumerator
            IEnumerable<TSlot> enumerable = _slots;
            return enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
