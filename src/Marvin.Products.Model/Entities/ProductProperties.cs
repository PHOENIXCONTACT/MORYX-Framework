// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Model;

namespace Marvin.Products.Model
{
    public class ProductProperties : ModificationTrackedEntityBase, IGenericColumns
    {
        public virtual int State { get; set; }

        public virtual long? ProductId { get; set; }

        public virtual ProductTypeEntity Product { get; set; }

        #region Flex fields

        public virtual long Integer1 { get; set; }
        public virtual long Integer2 { get; set; }
        public virtual long Integer3 { get; set; }
        public virtual long Integer4 { get; set; }
        public virtual long Integer5 { get; set; }
        public virtual long Integer6 { get; set; }
        public virtual long Integer7 { get; set; }
        public virtual long Integer8 { get; set; }
        public virtual double Float1 { get; set; }
        public virtual double Float2 { get; set; }
        public virtual double Float3 { get; set; }
        public virtual double Float4 { get; set; }
        public virtual double Float5 { get; set; }
        public virtual double Float6 { get; set; }
        public virtual double Float7 { get; set; }
        public virtual double Float8 { get; set; }
        public virtual string Text1 { get; set; }
        public virtual string Text2 { get; set; }
        public virtual string Text3 { get; set; }
        public virtual string Text4 { get; set; }
        public virtual string Text5 { get; set; }
        public virtual string Text6 { get; set; }
        public virtual string Text7 { get; set; }
        public virtual string Text8 { get; set; }

        #endregion
    }
}
