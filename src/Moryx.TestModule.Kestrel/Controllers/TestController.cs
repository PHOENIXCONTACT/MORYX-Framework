using Microsoft.AspNetCore.Mvc;
using Moryx.Container;

namespace Moryx.TestModule.Kestrel.Controllers
{
    [ApiController]
    [Component(LifeCycle.Transient)]
    public class TestController : Controller
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