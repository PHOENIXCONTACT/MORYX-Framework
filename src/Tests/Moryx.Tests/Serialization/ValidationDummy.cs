// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;

namespace Moryx.Tests.Serialization;

public class ValidationDummy
{
    public const int MinimumLength = 5;
    public const int MaximumLength = 10;
    public const string Regex = ".*";

    [MinLength(MinimumLength)]
    public string MinLenghtString { get; set; }

    [MaxLength(MaximumLength)]
    public string MaxLengthString { get; set; }

    [Base64String]
    public string Base64String { get; set; }

    [StringLength(MaximumLength, MinimumLength = MinimumLength)]
    public string MinMaxStringLenghtString { get; set; }

    [Length(MinimumLength, MaximumLength)]
    public string MinMaxLenghtString { get; set; }

    [RegularExpression(Regex)]
    public string RegexString { get; set; }

    [EmailAddress]
    public string EmailDataType { get; set; }

    [Required]
    public bool RequiredBool { get; set; }
}
