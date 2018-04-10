using System;
using Marvin.Model;
using System.Collections.Generic;

namespace Marvin.Products.Model
{
    public class ArticleEntity : EntityBase
    {
        public virtual long State { get; set; }
    
        public virtual DateTime ProductionDate { get; set; }
    
        public virtual string Identifier { get; set; }
    
        public virtual int NumberType { get; set; }
    
        public virtual long ProductId { get; set; }
    
        public virtual long? ParentId { get; set; }
    
        public virtual string ExtensionData { get; set; }

        public virtual long? PartLinkId { get; set; }

        public virtual ProductEntity Product { get; set; }
    
        public virtual ICollection<ArticleEntity> Parts { get; set; }

        public virtual ArticleEntity Parent { get; set; }
    
        public virtual PartLink PartLink { get; set; }
    }
}
