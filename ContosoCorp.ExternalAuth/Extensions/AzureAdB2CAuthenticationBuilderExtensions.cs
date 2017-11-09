﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Identity.Client;
using ContosoCorp.ExternalAuth.Models;

namespace Microsoft.AspNetCore.Authentication
{
    public static class AzureAdB2CAuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddAzureAdB2C(this AuthenticationBuilder builder)
            => builder.AddAzureAdB2C(_ => { });

        public static AuthenticationBuilder AddAzureAdB2C(this AuthenticationBuilder builder, Action<AzureAdB2COptions> configureOptions)
        {
            builder.Services.Configure(configureOptions);
            builder.Services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, ConfigureAzureOptions>();
            builder.AddOpenIdConnect();
            return builder;
        }

        private class ConfigureAzureOptions: IConfigureNamedOptions<OpenIdConnectOptions>
        {
            private readonly AzureAdB2COptions _azureOptions;

            public ConfigureAzureOptions(IOptions<AzureAdB2COptions> azureOptions)
            {
                _azureOptions = azureOptions.Value;
            }

            public void Configure(string name, OpenIdConnectOptions options)
            {
                options.ClientId = _azureOptions.ClientId;
                options.Authority = $"{_azureOptions.Instance}/{_azureOptions.Domain}/{_azureOptions.SignUpSignInPolicyId}/v2.0";
                options.UseTokenLifetime = true;
                options.CallbackPath = _azureOptions.CallbackPath;

                options.TokenValidationParameters = new TokenValidationParameters { NameClaimType = "name" };

                options.Events = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProvider = OnRedirectToIdentityProvider,
                    OnRemoteFailure = OnRemoteFailure,
                    OnAuthorizationCodeReceived = OnAuthroizationCodeReceivedAsync
                };
            }

            private async Task OnAuthroizationCodeReceivedAsync(AuthorizationCodeReceivedContext context)
            {
                var code = context.ProtocolMessage.Code;

                string signedInUserID = context.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;
                TokenCache userTokenCache = new MSALSessionCache(signedInUserID, context.HttpContext).GetMsalCacheInstance();
                ConfidentialClientApplication cca = new ConfidentialClientApplication(_azureOptions.ClientId, _azureOptions.Authority, "https://localhost:44337/"+$"{_azureOptions.CallbackPath}", new ClientCredential(_azureOptions.ClientSecret), userTokenCache, null);

                try
                {
                    AuthenticationResult result = await cca.AcquireTokenByAuthorizationCodeAsync(code, _azureOptions.ApiScopes.Split(' '));
                    context.HandleCodeRedemption(result.AccessToken, result.IdToken);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            public void Configure(OpenIdConnectOptions options)
            {
                Configure(Options.DefaultName, options);
            }

            public Task OnRedirectToIdentityProvider(RedirectContext context)
            {
                var defaultPolicy = _azureOptions.DefaultPolicy;
                if (context.Properties.Items.TryGetValue(AzureAdB2COptions.PolicyAuthenticationProperty, out var policy) && 
                    !policy.Equals(defaultPolicy))
                {
                    context.ProtocolMessage.Scope = OpenIdConnectScope.OpenIdProfile;
                    context.ProtocolMessage.ResponseType = OpenIdConnectResponseType.IdToken;
                    context.ProtocolMessage.IssuerAddress = context.ProtocolMessage.IssuerAddress.ToLower()
                        .Replace($"/{defaultPolicy.ToLower()}/", $"/{policy.ToLower()}/");
                    context.Properties.Items.Remove(AzureAdB2COptions.PolicyAuthenticationProperty);
                }
                else if (!string.IsNullOrEmpty(_azureOptions.ApiUrl))
                {
                    context.ProtocolMessage.Scope += $" offline_access {_azureOptions.ApiScopes}";
                    context.ProtocolMessage.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                }

                return Task.CompletedTask;
            }
 
            public Task OnRemoteFailure(RemoteFailureContext context)
            {
                context.HandleResponse();
                // Handle the error code that Azure AD B2C throws when trying to reset a password from the login page 
                // because password reset is not supported by a "sign-up or sign-in policy"
                if (context.Failure is OpenIdConnectProtocolException && context.Failure.Message.Contains("AADB2C90118"))
                {
                    // If the user clicked the reset password link, redirect to the reset password route
                    context.Response.Redirect("/Account/ResetPassword");
                }
                else if (context.Failure is OpenIdConnectProtocolException && context.Failure.Message.Contains("access_denied"))
                {
                    context.Response.Redirect("/");
                }
                else
                {
                    context.Response.Redirect("/Home/Error");
                }
                return Task.CompletedTask;
            }
        }
    }
}