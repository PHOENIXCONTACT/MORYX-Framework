﻿// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Configuration;
using Moryx.Serialization;
using Moryx.Workplans.WorkplanSteps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Moryx.Workplans.Endpoint
{
  /// <summary>
  /// Implementation of <see cref="ICustomSerialization"/> for types derived from <see cref="WorkplanStepBase"/>
  /// </summary>
  internal class WorkplanStepSerialization : PossibleValuesSerialization
  {
    public WorkplanStepSerialization() : base(null, new EmptyValueProvider())
    {
    }

    /// <summary>
    /// Only export properties flagged with <see cref="EntrySerializeAttribute"/>
    /// </summary>
    public override IEnumerable<PropertyInfo> GetProperties(Type sourceType)
    {
      return typeof(WorkplanStepBase).IsAssignableFrom(sourceType)
          ? base.GetProperties(sourceType).Where(p => CustomAttributeExtensions.GetCustomAttribute<EntrySerializeAttribute>((MemberInfo)p)?.Mode == EntrySerializeMode.Always)
          : new EntrySerializeSerialization().GetProperties(sourceType);
    }

    private class EmptyValueProvider : IEmptyPropertyProvider
    {
      public void FillEmpty(object obj)
      {
        ValueProviderExecutor.Execute(obj, new ValueProviderExecutorSettings().AddDefaultValueProvider());
      }
    }
  }

}

