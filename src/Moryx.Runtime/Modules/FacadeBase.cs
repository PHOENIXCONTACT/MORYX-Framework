// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Runtime.Modules
{
    /// <summary>
    /// Base implementation for a facade
    /// </summary>
    public class FacadeBase : IFacadeControl, ILifeCycleBoundFacade
    {
        private bool _activated;

        /// <inheritdoc />
        public Action ValidateHealthState { get; set; }

        /// <summary>
        /// Module is starting and facade activated
        /// </summary>
        public virtual void Activate()
        {
        }

        /// <inheritdoc />
        public virtual void Activated()
        {
            IsActivated = true;
        }

        /// <summary>
        /// Module is stopping and facade deactivated
        /// </summary>
        public virtual void Deactivate()
        {
        }

        /// <inheritdoc />
        public virtual void Deactivated()
        {
            IsActivated = false;
        }

        /// <inheritdoc />
        public bool IsActivated
        {
            get { return _activated; }
            set
            {
                if (_activated == value)
                    return;
                _activated = value;
                RaiseActivatedStateChanged(_activated);
            }
        }

        /// <inheritdoc />
        public event EventHandler<bool> StateChanged;

        private void RaiseActivatedStateChanged(bool newActivatedState)
        {
            StateChanged?.Invoke(this, newActivatedState);
        }
    }
}
