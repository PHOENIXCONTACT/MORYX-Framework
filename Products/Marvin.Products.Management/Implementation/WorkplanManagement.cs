using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Marvin.Container;
using Marvin.Model;
using Marvin.Products.Model;
using Marvin.Workflows;

namespace Marvin.Products.Management
{
    [Component(LifeCycle.Singleton, typeof(IWorkplanManagement))]
    internal class WorkplanManagement : IWorkplanManagement
    {
        #region Dependencies

        public IUnitOfWorkFactory ModelFactory { get; set; }

        #endregion

        public IWorkplan Create(string name)
        {
            using (var uow = ModelFactory.Create())
            {
                var wp = new Workplan
                {
                    Name = name,
                    Version = 1,
                    State = (int)WorkplanState.New
                };
                RecipeStorage.SaveWorkplan(uow, wp);

                uow.Save();

                return wp;
            }
        }

        public IReadOnlyList<Workplan> LoadAllWorkplans()
        {
            using (var uow = ModelFactory.Create())
            {
                var repo = uow.GetRepository<IWorkplanEntityRepository>();
                var workplans = (from entity in repo.Linq.Active()
                                 select new Workplan
                                 {
                                     Id = entity.Id,
                                     Name = entity.Name,
                                     Version = entity.Version,
                                     State = (WorkplanState)entity.State
                                 }).ToArray();
                return workplans;
            }
        }

        public Workplan LoadWorkplan(long workplanId)
        {
            using (var uow = ModelFactory.Create())
            {
                return RecipeStorage.LoadWorkplan(uow, workplanId);
            }
        }

        public long SaveWorkplan(Workplan workplan)
        {
            using (var uow = ModelFactory.Create())
            {
                var entity = RecipeStorage.SaveWorkplan(uow, workplan);

                uow.Save();

                return entity.Id;
            }
        }

        public void DeleteWorkplan(long workplanId)
        {
            using (var uow = ModelFactory.Create())
            {
                var repo = uow.GetRepository<IWorkplanEntityRepository>();
                var workplan = repo.GetByKey(workplanId);
                if (workplan == null || workplan.State != (int)WorkplanState.New)
                    return; // TODO: Any feedback?
                repo.Remove(workplan);
                uow.Save();

            }
        }
    }
}