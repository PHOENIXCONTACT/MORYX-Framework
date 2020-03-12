// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Marvin.Model
{
    internal class RemoveMethodStrategy : MethodProxyStrategyBase
    {
        public override bool CanImplement(MethodInfo methodInfo)
        {
            return false;
        }

        public override void Implement(TypeBuilder typeBuilder, MethodInfo methodInfo, Type baseType, Type targetType)
        {
            throw new NotImplementedException();
        }
    }
}
