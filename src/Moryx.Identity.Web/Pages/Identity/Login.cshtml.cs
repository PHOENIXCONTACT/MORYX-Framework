// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace Moryx.Identity.Web.Pages.Identity
{
    public class LoginModel : PageModel
    {
        private IConfiguration _configuration;

        public string LoginUrl { get; set; }

        public LoginModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult OnGet()
        {
            var identityUrl = _configuration["IdentityUrl"];
            if (identityUrl == null)
                return RedirectToPage("/IdentityError");

            LoginUrl = identityUrl + "/Login";
            return null;
        }
    }
}

