using System;
using Marvin.Products.UI.Interaction.InteractionSvc;

namespace Marvin.Products.UI.Interaction
{
    internal class ProductRevisionViewModel 
    {
        internal ProductRevisionEntry Model { get; set; }

        internal ProductRevisionViewModel(ProductRevisionEntry model)
        {
            Model = model;
        }

        public DateTime CreatedDate => Model.CreateDate;

        public DateTime? ReleaseDate => Model.ReleaseDate;

        public string Comment => Model.Comment;

        public short Revision => Model.Revision;

        public string State => Model.State.ToString();
    }
}
