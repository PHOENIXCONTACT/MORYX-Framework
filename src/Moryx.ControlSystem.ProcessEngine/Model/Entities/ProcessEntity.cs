#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Moryx.Model;

namespace Moryx.ControlSystem.ProcessEngine.Model
{
    public class ProcessEntity : ModificationTrackedEntityBase
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override long Id {
            get => base.Id;
            set => base.Id = value;
        }

        public virtual string TypeName { get; set; }

        /// <summary>
        /// Only used for production process
        /// </summary>
        public virtual long ReferenceId { get; set; }

        public virtual int State { get; set; }

        public virtual bool Rework { get; set; }

        #region Navigation properties

        public virtual ICollection<ActivityEntity> Activities { get; set; }

        public virtual ICollection<TokenHolderEntity> TokenHolders { get; set; }

        public virtual long? JobId { get; set; }

        public virtual JobEntity Job { get; set; }

        #endregion
    }
}
