// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Moryx.Model;

namespace Moryx.ControlSystem.ProcessEngine.Model;

public class JobEntity : ModificationTrackedEntityBase
{
    public virtual long RecipeId { get; set; }

    public virtual string RecipeProvider { get; set; }

    public virtual int Amount { get; set; }

    public virtual int State { get; set; }

    public virtual long? PreviousId { get; set; }

    #region Navigation properties

    public virtual ICollection<ProcessEntity> Processes { get; set; }

    public virtual JobEntity Previous { get; set; }

    #endregion
}