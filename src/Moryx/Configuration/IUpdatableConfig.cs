// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq.Expressions;

namespace Moryx.Configuration
{
    /// <summary>
    /// Event args passed to the config changed method
    /// </summary>
    public class ConfigChangedEventArgs : EventArgs
    {
        private readonly string[] _modifiedProperties;

        /// <summary>
        /// Create instance of the <see cref="ConfigChangedEventArgs"/> with all modified properties
        /// </summary>
        /// <param name="modifiedProperties">Names of modified properties</param>
        public ConfigChangedEventArgs(string[] modifiedProperties)
        {
            _modifiedProperties = modifiedProperties;
        }

        /// <summary>
        /// Check if this property was affected by the modification
        /// </summary>
        /// <param name="expression">Expression to determine property</param>
        /// <typeparam name="TProp">Type of the property</typeparam>
        /// <returns>True if property was affected</returns>
        public bool Contains<TProp>(Expression<Func<TProp>> expression)
        {
            var memberExp = (MemberExpression)expression.Body;
            var prop = memberExp.Member.Name;
            return _modifiedProperties.Contains(prop);
        }
    }

    /// <summary>
    /// Interface for all configs that support live update
    /// </summary>
    public interface IUpdatableConfig
    {
        /// <summary>
        /// Event raised when the config was modified
        /// </summary>
        event EventHandler<ConfigChangedEventArgs> ConfigChanged;

        /// <summary>
        /// External raise method invoked by <see cref="IConfigManager"/>
        /// </summary>
        /// <param name="modifiedProperties">Properties modified</param>
        void RaiseConfigChanged(params string[] modifiedProperties);
    }
}
