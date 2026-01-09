// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders.Restrictions;

/// <summary>
/// Restriction to limit possibility to report an operation
/// </summary>
public class ReportRestriction : IOperationRestriction
{
    /// <summary>
    /// Creates a new instance of <see cref="ReportRestriction"/>
    /// </summary>
    public ReportRestriction(bool canPartialReport, bool canFinalReport, RestrictionDescription description)
    {
        CanPartialReport = canPartialReport;
        CanFinalReport = canFinalReport;
        Description = description;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ReportRestriction"/>
    /// </summary>
    public ReportRestriction(bool canPartialReport, bool canFinalReport, string text, RestrictionSeverity severity)
        : this(canPartialReport, canFinalReport, new RestrictionDescription(text, severity))
    {
    }

    /// <summary>
    /// Indicator if a partial report is possible
    /// </summary>
    public bool CanPartialReport { get; }

    /// <summary>
    /// Indicator if a final report is possible
    /// </summary>
    public bool CanFinalReport { get; }

    /// <summary>
    /// Description of the results about rules which are not complied
    /// </summary>
    public RestrictionDescription Description { get; }
}