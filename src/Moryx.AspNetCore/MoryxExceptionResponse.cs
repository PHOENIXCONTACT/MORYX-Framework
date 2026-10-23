// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Microsoft.AspNetCore.Mvc;

namespace Moryx.AspNetCore;

public class MoryxExceptionResponse : ProblemDetails
{
    public string Exception { get; set; }
}
