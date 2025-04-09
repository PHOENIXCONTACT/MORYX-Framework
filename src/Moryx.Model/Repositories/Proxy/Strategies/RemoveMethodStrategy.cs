// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using System.Reflection.Emit;

namespace Moryx.Model.Repositories.Proxy
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
