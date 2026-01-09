// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Orders.Documents;

/// <summary>
/// Document class manufacturing cases
/// </summary>
[DataContract]
public abstract class Document
{
    /// <summary>
    /// Identifier of the document
    /// </summary>
    public string Identifier => Number + "-" + Revision.ToString("D2");

    /// <summary>
    /// Document number
    /// </summary>
    [DataMember]
    public string Number { get; set; }

    /// <summary>
    /// Current revision of the document
    /// </summary>
    [DataMember]
    public short Revision { get; set; }

    /// <summary>
    /// Information about the document like a long meaning full name
    /// </summary>
    [DataMember]
    public string Description { get; set; }

    /// <summary>
    /// Type of the document like a pdf with the laser picture
    /// </summary>
    [DataMember]
    public string Type { get; set; }

    /// <summary>
    /// Type of the file behind the given url
    /// </summary>
    [DataMember]
    public string ContentType { get; set; }

    /// <summary>
    /// Source of this document
    /// </summary>
    [DataMember]
    public string Source { get; set; }

    /// <summary>
    /// Constructor for the serialization
    /// </summary>
    protected Document()
    {
    }

    /// <summary>
    /// Default constructor
    /// </summary>
    protected Document(string number, short revision)
    {
        Number = number;
        Revision = revision;
    }

    /// <summary>
    /// Returns the stream to get the document file
    /// </summary>
    public abstract Stream GetStream();
}