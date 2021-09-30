// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Mvc;
using Moryx.Container;

namespace Moryx.TestModule.Kestrel
{
    [ApiController]
    [Component(LifeCycle.Transient)]
    internal class TestController : Controller
    {
        [HttpGet("foo/{value}")]
        [Produces("application/json")]
        public Foo GetFoo(int value) => new Foo {Value = value};
    }

    public class Foo
    {
        public int Value { get; set; }
    }
}