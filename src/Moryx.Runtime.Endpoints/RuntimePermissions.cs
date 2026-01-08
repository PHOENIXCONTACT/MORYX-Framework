// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Endpoints;

public static class RuntimePermissions
{
    private const string _prefix = "Moryx.Runtime.";
    private const string _databasePrefix = _prefix + "Database.";
    private const string _commonPrefix = _prefix + "Common.";
    private const string _modulesPrefix = _prefix + "Modules.";
    public const string DatabaseCanView = _databasePrefix + "CanView";
    public const string DatabaseCanSetAndTestConfig = _databasePrefix + "CanSetAndTestConfig";
    public const string DatabaseCanCreate = _databasePrefix + "CanCreate";
    public const string DatabaseCanErase = _databasePrefix + "CanErase";
    public const string DatabaseCanMigrateModel = _databasePrefix + "CanMigrateModel";
    public const string DatabaseCanSetup = _databasePrefix + "CanSetup";
    public const string CanGetGeneralInformation = _commonPrefix + "CanGetGeneralInformation";

    public const string ModulesCanView = _modulesPrefix + "CanView";
    public const string ModulesCanViewConfiguration = _modulesPrefix + "CanViewConfiguration";
    public const string ModulesCanViewMethods = _modulesPrefix + "CanViewMethods";
    public const string ModulesCanControl = _modulesPrefix + "CanControl";
    public const string ModulesCanConfigure = _modulesPrefix + "CanConfigure";
    public const string ModulesCanConfirmNotifications = _modulesPrefix + "CanConfirmNotifications";
    public const string ModulesCanInvoke = _modulesPrefix + "CanInvoke";
}