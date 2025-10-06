// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.VisualInstructions
{
    /// <summary>
    /// Attribute to be used on enum members to declare its usage for assembly instruction results
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumInstructionAttribute : Attribute
    {
        /// <summary>
        /// If set to  <c>true</c>, this enum value marked with this attribute will not be used to display a button.
        /// </summary>
        public bool Hide { get; set; }

        /// <summary>
        /// Create instruction attribute without title
        /// </summary>
        public EnumInstructionAttribute()
        {
            
        }
    }
}
