﻿// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Drawing;

namespace Moryx.Workplans
{
    /// <summary>
    /// Session 
    /// </summary>
    public class WorkplanSession
    {
        /// <summary>
        /// Token of the session
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Workplan that is edited
        /// </summary>
        public Workplan Workplan { get; set; }
    }
}
