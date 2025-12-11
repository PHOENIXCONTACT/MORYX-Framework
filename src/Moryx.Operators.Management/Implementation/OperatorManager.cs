// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Model.Repositories;
using Moryx.Operators.Management.Model;
using Moryx.AbstractionLayer.Resources;
using Moryx.Model;

namespace Moryx.Operators.Management;

[Component(LifeCycle.Singleton, typeof(IOperatorManager))]
internal class OperatorManager : IOperatorManager
{
    #region Dependencies
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public IUnitOfWorkFactory<OperatorsContext> UnitOfWorkFactory { get; set; }

    public IResourceManagement ResourceManagement { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    #endregion

    #region IOperatorManager

    private readonly List<OperatorData> _operators = [];

    public IReadOnlyList<OperatorData> Operators => _operators;

    public OperatorData Add(Operator @operator)
    {
        using var uow = UnitOfWorkFactory.Create();
        var data = new OperatorData(@operator.Identifier)
        {
            FirstName = @operator.FirstName,
            LastName = @operator.LastName,
            Pseudonym = @operator.Pseudonym,
        };

        if (@operator is AssignableOperator assignableOperator)
        {
            data.AssignedResources.AddRange(assignableOperator.AssignedResources);
        }

        var entity = OperatorStorage.Save(uow, data);
        data.Id = entity.Id;
        _operators.Add(data);
        OperatorChanged?.Invoke(this, new OperatorChangedEventArgs(data.Operator) { Change = OperatorChange.Creation });
        return data;
    }

    public void Update(Operator @operator)
    {
        using var uow = UnitOfWorkFactory.Create();

        var toBeUpdated = _operators.First(o => o.Identifier == @operator.Identifier);
        toBeUpdated.Identifier = @operator.Identifier;
        toBeUpdated.FirstName = @operator.FirstName;
        toBeUpdated.LastName = @operator.LastName;
        toBeUpdated.Pseudonym = @operator.Pseudonym;
        if (@operator is AssignableOperator assignableOperator
            && !toBeUpdated.AssignedResources.SequenceEqual(assignableOperator.AssignedResources))
        {
            toBeUpdated.AssignedResources.Clear();
            toBeUpdated.AssignedResources.AddRange(assignableOperator.AssignedResources);
        }
        OperatorStorage.Save(uow, toBeUpdated);
        OperatorChanged?.Invoke(this, new OperatorChangedEventArgs(toBeUpdated.Operator) { Change = OperatorChange.Update });
    }

    public void Delete(string identifier)
    {
        using var uow = UnitOfWorkFactory.Create();
        var @operator = _operators.First(o => o.Identifier == identifier);
        OperatorStorage.Delete(uow, @operator);
        _operators.Remove(@operator);
        OperatorChanged?.Invoke(this, new OperatorChangedEventArgs(@operator.Operator) { Change = OperatorChange.Deletion });
    }

    public event EventHandler<OperatorChangedEventArgs>? OperatorChanged;

    #endregion

    #region IPlugin

    public void Start()
    {
        using var uow = UnitOfWorkFactory.Create();
        var operatorRepo = uow.GetRepository<IOperatorEntityRepository>();
        var restored = operatorRepo.Linq.Active().ToArray()
            .Select(e => OperatorStorage.Load(e, ResourceManagement));

        _operators.AddRange(restored);
    }

    public void Stop()
    {
        _operators.Clear();
    }

    #endregion
}
