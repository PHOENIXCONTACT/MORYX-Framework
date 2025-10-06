// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;

namespace Moryx.Tools
{
    /// <summary>
    /// Static platfrom library to compose the app domain
    /// </summary>
    public static class AppDomainBuilder
    {
        /// <summary>
        /// Load all assemblies from directory
        /// </summary>
        public static void LoadAssemblies()
        {
            var parentDir = AppDomain.CurrentDomain.BaseDirectory;
            var assemblies = Directory.GetFiles(parentDir, "*.dll", SearchOption.TopDirectoryOnly);
            foreach (var assemblyFile in assemblies)
            {
                if (AppDomain.CurrentDomain.GetAssemblies().Any(loadedAssembly => NamesMatch(loadedAssembly, assemblyFile)))
                    continue;

                try
                {
                    Assembly.LoadFrom(assemblyFile);
                }
                catch (Exception ex)
                {
                    var loadEx = new FileLoadException("Failed to load assembly file " + assemblyFile, ex);
                    Console.WriteLine(ExceptionPrinter.Print(loadEx));
                }
            }
        }

        /// <summary>
        /// Find a matching assembly in the AppDomain
        /// </summary>
        public static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(item => item.GetName().Name == args.Name.Split(',')[0]);
        }

        private static bool NamesMatch(Assembly loadedAssembly, string assemblyFile)
        {
            var assemblyName = loadedAssembly.GetName().Name;
            return assemblyName == Path.GetFileNameWithoutExtension(assemblyFile);
        }
    }
}

