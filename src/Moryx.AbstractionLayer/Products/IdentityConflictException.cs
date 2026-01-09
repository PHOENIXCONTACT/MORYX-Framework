// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Properties;

namespace Moryx.AbstractionLayer.Products;

/// <summary>
/// Exception thrown during product duplication when the new identity has conflicts with existing products
/// OR the given template can not be used create products of the new identity
/// </summary>
public class IdentityConflictException : Exception
{
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
    public IdentityConflictException(bool invalidTemplate)
        : base(invalidTemplate ? Strings.IdentityConflictException_InvalidTemplateMessage : Strings.IdentityConflictException_IdentityConflictMessage)
    {
        InvalidTemplate = invalidTemplate;
    }
}