// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.FactoryMonitor.Endpoints.Models
{
    /// <summary>
    /// A graph that represent the factory and the Resources that should be displayable in it.
    /// </summary>
    public class SimpleFactoryGraph
    {

        /// <summary>
        /// Parent factory ID.
        /// </summary>
        /// The current factory can be inside another factory
        public long ParentId { get;  set; }

        public long CurrentId { get;  set; }
        /// <summary>
        /// Specify if this is a cell.
        /// An item on the UI can be a Cell or a Factory
        /// </summary>
        public bool IsACellLocation { get;  set; }
        /// <summary>
        /// Holds the name of the type of the Item (Cell,Factory,Location,etc...)
        /// </summary>
        public string Type {  get; set; }
        /// <summary>
        /// Contains the elements that should be displayed in the current factory
        /// </summary>
        public List<SimpleFactoryGraph> Children { get;  set; } = new List<SimpleFactoryGraph>();
    }
}

