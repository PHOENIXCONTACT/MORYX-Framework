// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Products.Model
{
    /// <summary>
    /// Shared interface for all entities, that offer additional generic columns for derived applications
    /// </summary>
    public interface IGenericColumns
    {
        long Integer1 { get; set; }
        long Integer2 { get; set; }
        long Integer3 { get; set; }
        long Integer4 { get; set; }
        long Integer5 { get; set; }
        long Integer6 { get; set; }
        long Integer7 { get; set; }
        long Integer8 { get; set; }
        double Float1 { get; set; }
        double Float2 { get; set; }
        double Float3 { get; set; }
        double Float4 { get; set; }
        double Float5 { get; set; }
        double Float6 { get; set; }
        double Float7 { get; set; }
        double Float8 { get; set; }
        string Text1 { get; set; }
        string Text2 { get; set; }
        string Text3 { get; set; }
        string Text4 { get; set; }
        string Text5 { get; set; }
        string Text6 { get; set; }
        string Text7 { get; set; }
        string Text8 { get; set; }
    }
}
