// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Linq;
using Marvin.Container;
using Marvin.Model;
using Marvin.Products.Model;
using Marvin.Workflows;

namespace Marvin.Products.Management
{
    [Component(LifeCycle.Singleton, typeof(IWorkplans))]
    internal class WorkplanManagement : IWorkplans
    {
        #region Dependencies

        public IUnitOfWorkFactory ModelFactory { get; set; }

        #endregion

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
                if (workplan == null)
                    return; // TODO: Any feedback?
                repo.Remove(workplan);
                uow.Save();

            }
        }
    }
}
