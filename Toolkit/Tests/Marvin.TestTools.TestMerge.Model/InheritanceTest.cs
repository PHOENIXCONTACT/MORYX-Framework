using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devart.Common;
using Marvin.Model;
using Marvin.TestTools.Test.Model;

namespace Marvin.TestTools.TestMerge.Model
{
    internal class InheritanceTemplateTest
    {
        public IUnitOfWorkFactory UnitOfWorkFactory { get; set; }

        private void UsingMethodsPlayground()
        {
            using (var context = UnitOfWorkFactory.Create())
            {
                var aRepo = context.GetRepository<ITopParentRepository>();
                var mergeRepo = context.GetRepository<IMergedChildTPT1Repository>();

                aRepo.GetAll(false);
                mergeRepo.GetAll(false);

                aRepo.GetAllByUpdated(DateTime.Now);
                mergeRepo.GetAllByUpdated(DateTime.Now);

                aRepo.GetByNumber(1);
                mergeRepo.GetByNumber(1);

                aRepo.GetCreatedAndUpdated(DateTime.MinValue, DateTime.MaxValue);
                mergeRepo.GetCreatedAndUpdated(DateTime.MinValue, DateTime.MaxValue);

                aRepo.GetUpdatedAndNumber(DateTime.Now, 1);
                mergeRepo.GetUpdatedAndNumber(DateTime.Now, 1);
            }
        }
    }
}
