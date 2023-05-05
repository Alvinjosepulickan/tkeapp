using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Authentication
{
	public static class OpenIdAuthenticationBuilderExtensions
	{
        public static AuthenticationBuilder AddOpenIdSchemes(this AuthenticationBuilder builder, IConfigurationSection configureOptions)
        {
            List<OpenIdProvider> providers = configureOptions.Get<List<OpenIdProvider>>();

			foreach (var provider in providers)
			{
				builder.AddOpenIdConnect(provider.AuthenticationScheme, provider.DisplayName, options =>
				 {
					 options.ClientId = provider.ClientId;
					 options.Authority = $"{provider.Instance}{provider.TenantId}";
					 options.UseTokenLifetime = true;
					 options.CallbackPath = provider.CallbackPath;
					 options.RequireHttpsMetadata = false;
					 //options.NonceCookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
					 //options.CorrelationCookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
					 options.SaveTokens = true;
				 });
			}

			return builder;
        }
    }

    public class OpenIdProvider
    {
		public string AuthenticationScheme { get; set; }
		public string DisplayName { get; set; }
		public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Instance { get; set; }
        public string Domain { get; set; }
        public string TenantId { get; set; }
        public string CallbackPath { get; set; }
    }
}
