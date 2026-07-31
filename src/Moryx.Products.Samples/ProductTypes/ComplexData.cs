// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Products.Samples;


/// <summary>
/// To test complex data in text columns
/// </summary>
public class ComplexData
{
    // Property name "Name" is reserved: do not use!
    // Try to analyse SMAF-5082
    public string Name { get; set; }

    public string Content { get; set; }
   
    public string PropertyName { get; set; }

    public int Number { get; set; }

    public float Weight { get; set; }
}

