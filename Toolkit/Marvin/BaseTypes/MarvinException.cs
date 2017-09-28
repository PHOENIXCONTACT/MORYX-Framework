using System;
using System.Runtime.Serialization;

namespace Marvin
{
    /// <summary>
    /// BaseClass for exceptions within MARVIN code. It should add usable information to the System.Exception, 
    /// that may be used by a superior component/module/plugin to handle the exception.
    /// </summary>
    public abstract class MarvinException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarvinException"/> class.
        /// </summary>
        protected MarvinException() 
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance with serialized data.
        /// </summary>
        /// <param name="si">The SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
        public MarvinException(SerializationInfo si, StreamingContext context) : base(si, context)
        {
            DisplayText = (string)si.GetValue("DisplayText", typeof(string));
            NotificationKey = (string)si.GetValue("NotificationKey", typeof(string));
            UserRelevant = (bool)si.GetValue("UserRelevant", typeof(bool));
            IsFatal = (bool)si.GetValue("IsFatal", typeof(bool));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarvinException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        protected MarvinException(string message)
            : this(message, null)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarvinException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        protected MarvinException(string message, Exception innerException)
            : this(message, innerException, false, false, string.Empty)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarvinException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="userRelevant">if set to <c>true</c> [user relevant].</param>
        /// <param name="isFatal">if set to <c>true</c> [is fatal].</param>
        /// <param name="displayText">The display text.</param>
        protected MarvinException(string message, Exception innerException, bool userRelevant, bool isFatal, string displayText)
            : base(message, innerException)
        {
            DisplayText = displayText;
            UserRelevant = userRelevant;
            IsFatal = isFatal;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarvinException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="userRelevant">if set to <c>true</c> [user relevant].</param>
        /// <param name="isFatal">if set to <c>true</c> [is fatal].</param>
        /// <param name="displayText">The display text.</param>
        /// <param name="notificationKey">The notification key.</param>
        protected MarvinException(string message, Exception innerException, bool userRelevant, bool isFatal, string displayText, string notificationKey)
            : base(message, innerException)
        {
            DisplayText = displayText;
            NotificationKey = notificationKey;
            UserRelevant = userRelevant;
            IsFatal = isFatal;
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

            info.AddValue("DisplayText", DisplayText);
            info.AddValue("NotificationKey", NotificationKey);
            info.AddValue("UserRelevant", UserRelevant);
            info.AddValue("IsFatal", IsFatal);
        }

        /// <summary>
        /// Gets or sets the display text.
        /// </summary>
        /// <value>
        /// The display text.
        /// </value>
        public string DisplayText { get; set; }

        /// <summary>
        /// The notification key to be used be AbstractionLayer's ExceptionProcessor
        /// </summary>
        public string NotificationKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this exception is user relevant.
        /// </summary>
        /// <value>
        ///   <c>true</c> if user relevant; otherwise, <c>false</c>.
        /// </value>
        public bool UserRelevant { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this exception is fatal for the throwing component/module/object.
        /// </summary>
        /// <value>
        ///   <c>true</c> if fatal; otherwise, <c>false</c>.
        /// </value>
        public bool IsFatal { get; set; }
    }
}
