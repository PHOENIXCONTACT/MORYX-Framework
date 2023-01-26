// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ComponentModel;

namespace Moryx.Model
{
    /// <summary>
    /// Listener of the <see cref="INotifyPropertyChanged.PropertyChanged"/> event for
    /// <see cref="IEntity.Id"/>.
    /// </summary>
    public abstract class EntityIdListener
    {
        /// <summary>
        /// Listen to id change and invoke callback.
        /// </summary>
        public static void Listen(IEntity entity, Action<long> idCallback)
        {
            Listen(entity, new CallbackListener(idCallback));
        }

        /// <summary>
        /// Listen to id change and set on <see cref="IPersistentObject.Id"/>.
        /// </summary>
        public static void Listen(IEntity entity, IPersistentObject target)
        {
            Listen(entity, new PersistentListener(target));
        }

        /// <summary>
        /// Listen to id change and assign to a merged entity.
        /// </summary>
        public static void Listen(IEntity entity, IEntity target)
        {
            Listen(entity, new DoubleEntityListener(target));
        }

        /// <summary>
        /// Listen to entity change using a custom listener implementation.
        /// </summary>
        public static void Listen(IEntity entity, EntityIdListener listener)
        {
            // If id is already set, take a shortcut and assign directly
            if (entity.Id > 0)
            {
                listener.AssignId(entity.Id);
            }
            else
            {
                entity.IdChanged += listener.IdChanged;
            }
        }

        private void IdChanged(object sender, EventArgs eventArgs)
        {
            var entity = (IEntity)sender;
            entity.IdChanged -= IdChanged;
            AssignId(entity.Id);
        }

        /// <summary>Assign id using derived strategy.</summary>
        protected abstract void AssignId(long id);

        #region Listener Strategies

        private class CallbackListener : EntityIdListener
        {
            private readonly Action<long> _idCallback;

            public CallbackListener(Action<long> idCallback)
            {
                _idCallback = idCallback;
            }

            protected override void AssignId(long id)
            {
                _idCallback(id);
            }
        }

        private class PersistentListener : EntityIdListener
        {
            private readonly IPersistentObject _target;

            public PersistentListener(IPersistentObject target)
            {
                _target = target;
            }

            protected override void AssignId(long id)
            {
                _target.Id = id;
            }
        }

        private class DoubleEntityListener : EntityIdListener
        {
            private readonly IEntity _target;

            public DoubleEntityListener(IEntity target)
            {
                _target = target;
            }

            protected override void AssignId(long id)
            {
                _target.Id = id;
            }
        }

        #endregion
    }
}
