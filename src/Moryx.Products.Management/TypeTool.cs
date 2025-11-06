// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Tools;

namespace Moryx.Products.Management
{
    internal static class TypeTool
    {
        public static object CreateInstance<TTpye>(string typeName)
        {
            var type = ReflectionTool.GetPublicClasses<TTpye>(t => t.Name == typeName).FirstOrDefault();
            if (type == null)
                return null;
            var obj = Activator.CreateInstance(type);
            return obj;
        }
    }
}

