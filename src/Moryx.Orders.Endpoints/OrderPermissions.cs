// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders.Endpoints;

public static class OrderPermissions
{
    private const string _prefix = "Moryx.Orders.";
    public const string CanView = _prefix + "CanView";
    public const string CanViewDocuments = _prefix + "CanViewDocuments";
    public const string CanAdd = _prefix + "CanAdd";
    public const string CanManage = _prefix + "CanManage";
    public const string CanBegin = _prefix + "CanBegin";
    public const string CanInterrupt = _prefix + "CanInterrupt";
    public const string CanReport = _prefix + "CanReport";
    public const string CanAdvice = _prefix + "CanAdvice";
}