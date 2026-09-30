// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace StartProject.Asp;

public class ExamplePolicyProvider(IOptions<AuthorizationOptions> options) : DefaultAuthorizationPolicyProvider(options)
{
    public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName) => await base.GetPolicyAsync(policyName)
            ?? new AuthorizationPolicyBuilder().RequireClaim("Permission", policyName).Build();
}
