// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace Moryx.Identity.Web.Pages
{
    public class IdentityModel : PageModel
    {
        private IConfiguration _configuration;

        public string IdentityUrl { get; set; }

        public IdentityModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult OnGet()
        {
            var identityUrl = _configuration["IdentityUrl"];
            if (identityUrl == null)
                return RedirectToPage("/IdentityError");

            IdentityUrl = identityUrl;
            return null;
        }
    }
}

