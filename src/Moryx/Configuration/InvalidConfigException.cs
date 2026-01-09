// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Configuration;

/// <summary>
/// Exception 
/// </summary>
public class InvalidConfigException : Exception
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public InvalidConfigException()
    {
    }

    /// <summary>
    /// Signal an invalid value within the configuration
    /// </summary>
    /// <param name="faultyEntry">Optional subentry defining the faulty property</param>
    /// <param name="propertyFailure">Failure description containing property name and cause of failure</param>
    public InvalidConfigException(object faultyEntry, string propertyFailure)
        : base($"Invalid entry {faultyEntry.GetType().Name} in config. Error: {propertyFailure}")
    {
        FaultyEntry = faultyEntry;
        PropertyFailure = propertyFailure;
    }

    /// <summary>
    /// Optional subentry defining the faulty property
    /// </summary>
    public object FaultyEntry { get; set; }

    /// <summary>
    /// Failure description containing property name and cause of failure
    /// </summary>
    public string PropertyFailure { get; set; }
}