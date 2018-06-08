using System.ComponentModel.DataAnnotations;
using Marvin.Model;

namespace Marvin.Products.Model
{
    public class ProductDocument : ModificationTrackedEntityBase
    {
        [MaxLength(byte.MaxValue)]
        public virtual string FileName { get; set; }

        [MaxLength(byte.MaxValue)]
        public virtual string FilesystemPath { get; set; }

        public virtual byte[] File { get; set; }

        public virtual int Version { get; set; }

        [MaxLength(ushort.MaxValue)]
        public virtual string Hash { get; set; }

        public virtual long ProductId { get; set; }

        public virtual ProductEntity Product { get; set; }
    }
}
