using System;
using System.Runtime.Serialization;

namespace Marvin.Runtime.Kernel.ModuleManagement
{
    internal class MissingFacadeException : MarvinException
    {
        public string ModuleName { get; set; }
        public string PropName { get; set; }
        public Type FacadeType { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MissingFacadeException()
        {
        }

        /// <summary>
        /// Initializes a new instance with serialized data.
        /// </summary>
        /// <param name="si">The SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
        public MissingFacadeException(SerializationInfo si, StreamingContext context)
            : base(si, context)
        {
            ModuleName = (string)si.GetValue("ModuleName", typeof(string));
            PropName = (string)si.GetValue("PropName", typeof(string));
            FacadeType = (Type)si.GetValue("FacadeType", typeof(Type));
        }

        public MissingFacadeException(string moduleName, string propName, Type facadeType)
            : base(string.Format("Found no plugin hosting a facade of type {0} which was expected by {1}.{2}", facadeType.Name, moduleName, propName))
        {
            ModuleName = moduleName;
            PropName = propName;
            FacadeType = facadeType;
        }

        /// <summary>
        /// When overridden in a derived class, sets the System.Runtime.Serialization.SerializationInfo
        ///  with information about the exception.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialized 
        /// object data about the exception being thrown.</param>
        /// <param name="context">The System.Runtime.Serialization.StreamingContext that contains contextual
        /// information about the source or destination.</param>
        /// <exception cref="System.ArgumentNullException">The info parameter is a null reference</exception>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("ModuleName", ModuleName);
            info.AddValue("PropName", PropName);
            info.AddValue("FacadeType", FacadeType);
        }
    }
}
