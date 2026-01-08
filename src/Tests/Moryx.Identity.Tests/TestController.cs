// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Moryx.Identity.Tests
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoAuthClassController : ControllerBase
    {
        [HttpGet("NoAuthMethod")]
        public IEnumerable<string> NoAuthMethod()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("AuthMethod")]
        [Authorize]
        public IEnumerable<string> AuthMethod()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("SpecificAuthMethod")]
        [Authorize(Policy = "Need.Specific.Permission")]
        public IEnumerable<string> SpecificAuthMethod()
        {
            return new string[] { "value1", "value2" };
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthClassController : ControllerBase
    {
        [HttpGet("NoAuthMethod")]
        public IEnumerable<string> NoAuthMethod()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("SpecificAuthMethod")]
        [Authorize(Policy = "Need.Specific.Permission")]
        public IEnumerable<string> SpecificAuthMethod()
        {
            return new string[] { "value1", "value2" };
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Need.Specific.Permission")]
    public class SpecificAuthClassController : ControllerBase
    {
        [HttpGet("AnonymousMethod")]
        [AllowAnonymous]
        public IEnumerable<string> AnonymousMethod()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("NoAuthMethod")]
        public IEnumerable<string> NoAuthMethod()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("AuthMethod")]
        [Authorize]
        public IEnumerable<string> AuthMethod()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("SpecificAuthMethod")]
        [Authorize(Policy = "Need.Another.Specific.Permission")]
        public IEnumerable<string> SpecificAuthMethod()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
