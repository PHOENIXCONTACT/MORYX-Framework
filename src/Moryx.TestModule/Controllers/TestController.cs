// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Mvc;

namespace Moryx.TestModule.Kestrel
{
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("foo/{value}")]
        [Produces("application/json")]
        public Foo GetFoo(int value) => new() { Value = value };
    }

    public class Foo
    {
        public int Value { get; set; }
    }
}