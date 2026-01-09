// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Resources;

/// <summary>
/// Attribute used to decorate a property that references another resource
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ResourceReferenceAttribute : Attribute
{
    /// <summary>
    /// Name of the reference
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Type of relation with the referenced resource
    /// </summary>
    public ResourceRelationType RelationType { get; }

    /// <summary>
    /// Role of the referenced resource in the relation
    /// </summary>
    public ResourceReferenceRole Role { get; }

    /// <summary>
    /// Automatically save changes to the collection of references
    /// </summary>
    public bool AutoSave { get; set; }

    /// <summary>
    /// Reference is required and must be set
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Create a typed reference with <see cref="ResourceReferenceRole.Target"/>
    /// </summary>
    /// <param name="relationType">Type of the reference</param>
    public ResourceReferenceAttribute(ResourceRelationType relationType)
        : this(relationType, ResourceReferenceRole.Target, null)
    {
    }

    /// <summary>
    /// Create a typed reference with <see cref="ResourceReferenceRole.Target"/>
    /// </summary>
    /// <param name="relationType">Type of the reference</param>
    /// <param name="name">Name of the reference if there is more than one</param>
    public ResourceReferenceAttribute(ResourceRelationType relationType, string name)
        : this(relationType, ResourceReferenceRole.Target, name)
    {
    }

    /// <summary>
    /// Create a named resource reference of type <see cref="ResourceRelationType.Custom"/>. This
    /// is also the default behavior if no attribute is defined.
    /// </summary>
    public ResourceReferenceAttribute(string relationName)
        : this(ResourceRelationType.Custom, ResourceReferenceRole.Target, relationName)
    {
    }

    /// <summary>
    /// Create a typed reference and specify the role explicitly
    /// </summary>
    /// <param name="relationType">Type of the reference</param>
    /// <param name="role">Role of the reference in the relation</param>
    public ResourceReferenceAttribute(ResourceRelationType relationType, ResourceReferenceRole role)
        : this(relationType, role, null)
    {
    }

    /// <summary>
    /// Create a typed and named reference with a special name
    /// </summary>
    public ResourceReferenceAttribute(ResourceRelationType relationType, ResourceReferenceRole role, string name)
    {
        Role = role;
        Name = name;
        RelationType = relationType;
    }
}