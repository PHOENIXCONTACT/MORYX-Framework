using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Marvin.Model
{
    internal interface IMethodProxyStrategy
    {
        bool CanImplement(MethodInfo methodInfo);

        void Implement(TypeBuilder typeBuilder, MethodInfo methodInfo, Type baseType, Type targetType);
    }
}