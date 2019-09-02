using System;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Exception thrown during product duplication when the new identity has conflicts with existing products
    /// OR the given template can not be used create products of the new identity
    /// </summary>
    public class IdentityConflictException : Exception
    {
        // TODO: Localize exception messages
        private const string IdentityConflictMessage = "The identity has conflicts with existing products and revisions!";

        private  const string InvalidTemplateMessage = "The identity can not be used for duplicates of the template!";

        /// <summary>
        /// Indicates that the template is incompatible with the given identity
        /// </summary>
        public bool InvalidTemplate { get; }

        /// <summary>
        /// Create a new <see cref="IdentityConflictException"/>
        /// </summary>
        public IdentityConflictException() : this(false)
        {
        }

        /// <summary>
        /// The given template product can not be used to create types of the given identity
        /// </summary>
        /// <param name="invalidTemplate"></param>
        public IdentityConflictException(bool invalidTemplate) : base(invalidTemplate ? InvalidTemplateMessage : IdentityConflictMessage)
        {
            InvalidTemplate = invalidTemplate;
        }
    }
}