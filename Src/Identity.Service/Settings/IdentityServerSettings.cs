﻿using Duende.IdentityServer.Models;

namespace Identity.Service.Settings;

public class IdentityServerSettings
{
    public IReadOnlyCollection<ApiScope> ApiScopes { get; init; } = Array.Empty<ApiScope>();

    public IReadOnlyCollection<Client> Clients { get; init; } = Array.Empty<Client>();

    public IReadOnlyCollection<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId()
        };
}