// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans.Web.Models;

public class OpenSessionRequest
{
    public long WorkplanId { get; set; }

    public bool Duplicate { get; set; }
}