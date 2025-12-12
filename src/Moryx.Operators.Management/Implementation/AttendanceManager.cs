// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Model.Repositories;
using Moryx.Operators.Attendances;
using Moryx.Operators.Management.Model;

namespace Moryx.Operators.Management;

[Component(LifeCycle.Singleton, typeof(IAttendanceManager))]
internal class AttendanceManager : IAttendanceManager
{
    #region Dependencies
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public IUnitOfWorkFactory<OperatorsContext> UnitOfWorkFactory { get; set; }

    public ModuleConfig ModuleConfig { get; set; }

    public IOperatorManager OperatorManager { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    #endregion

    #region IAttandanceManager

    public OperatorData? DefaultOperator { get; private set; }

    public void SignIn(OperatorData operatorData, IOperatorAssignable resource)
    {
        if (operatorData.AssignedResources.Any(r => r.Id == resource.Id))
            return;

        operatorData.AssignedResources.Add(resource);
        OperatorManager.Update(operatorData.Operator);
        OperatorSignedIn?.Invoke(this, operatorData);
    }

    public void SignOut(OperatorData operatorData, IOperatorAssignable resource)
    {
        if (!operatorData.AssignedResources.Remove(resource))
            return;
        OperatorManager.Update(operatorData.Operator);
        OperatorSignedOut?.Invoke(this, operatorData);
    }

#pragma warning disable CS8618 // Facade always subscribes
    public event EventHandler<OperatorData> OperatorSignedIn;

    public event EventHandler<OperatorData> OperatorSignedOut;
#pragma warning restore CS8618 // Facade always subscribes

    #endregion

    #region IPlugin

    public void Start()
    {
        using var uow = UnitOfWorkFactory.Create();
        DefaultOperator = GetOrCreateDefaultOperator();
    }

    private OperatorData? GetOrCreateDefaultOperator()
    {
        if (string.IsNullOrEmpty(ModuleConfig.DefaultOperator))
            return null;

        var defaultOperatorData = OperatorManager.Operators.FirstOrDefault(o => o.Identifier.Equals(ModuleConfig.DefaultOperator));
        if (defaultOperatorData != null)
            return defaultOperatorData;

        var defaultOperator = new Operator(ModuleConfig.DefaultOperator)
        {
            FirstName = "Marjory",
            LastName = "Stewart-Baxter",
            Pseudonym = "MS2024"
        };

        defaultOperatorData = OperatorManager.Add(defaultOperator);

        return defaultOperatorData;
    }

    public void Stop()
    {
        DefaultOperator = null;
    }

    #endregion
}
