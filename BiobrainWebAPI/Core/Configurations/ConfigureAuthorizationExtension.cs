using System;
using System.IdentityModel.Tokens.Jwt;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Domain.Entities.SiteIdentity;
using BiobrainWebAPI.Authorization;
using BiobrainWebAPI.Values.Options;
using DataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;

namespace BiobrainWebAPI.Core.Configurations
{
    public static class ConfigureAuthorizationExtension
    {
        public static IServiceCollection ConfigureJwtIdentity(this IServiceCollection service, IConfiguration configuration)
        {
            var settings = service.AddBearerSettings(configuration);

            service.AddServices()
                   .AddIdentity()
                   .AddOpenIdServer(settings)
                   .AddJwtAuthorization()
                   .AddJwtAuthentication(settings);

            return service;
        }

        private static BearerSettings AddBearerSettings(this IServiceCollection service, IConfiguration configuration)
        {
            var configurationSection = configuration.GetSection("BearerSettings");
            service.Configure<BearerSettings>(configurationSection);

            return configurationSection.Get<BearerSettings>();
        }

        private static IServiceCollection AddServices(this IServiceCollection service)
        {
            service.AddScoped<ISessionContext, SessionContext>();
            service.AddScoped<IUserAuthorizationService, UserAuthorizationService>();
            
            return service;
        }
        
        private static IServiceCollection AddIdentity(this IServiceCollection service)
        {
            // Register the Identity services.
            service.AddIdentity<UserEntity, RoleEntity>(options =>
                   {
                       options.Password.RequiredLength = 6;
                       options.Password.RequireLowercase = false;
                       options.Password.RequireNonAlphanumeric = false;
                       options.Password.RequireDigit = false;
                       options.Password.RequireUppercase = false;

                       options.User.RequireUniqueEmail = true;
                   })
                   .AddEntityFrameworkStores<BiobrainWebContext>()
                   .AddDefaultTokenProviders();

            service.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
            });

            return service;
        }

        private static IServiceCollection AddOpenIdServer(this IServiceCollection service, BearerSettings settings)
        {
            service.AddOpenIddict()
                   .AddCore(options =>
                   {
                       options.UseEntityFrameworkCore()
                              .UseDbContext<BiobrainWebContext>()
                              .ReplaceDefaultEntities<Guid>();
                   })

                   // Register the OpenIddict server components.
                   .AddServer(builder =>
                   {
                       builder.SetTokenEndpointUris("/api/connect/token", "/api/auth/saml/exchange");
                       var issuer = new Uri(settings.Authority);
                       builder.SetIssuer(issuer);

                       builder.AllowPasswordFlow()
                              .AllowRefreshTokenFlow();

                       builder.AcceptAnonymousClients();
                       builder.DisableAccessTokenEncryption();

                       // Register the signing and encryption credentials.
                       // NOTE: Development certificates disabled on macOS (keychain access issue).
                       // Ephemeral keys below are used instead for local dev.
                       // builder.AddDevelopmentEncryptionCertificate()
                       //        .AddDevelopmentSigningCertificate();

                       // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                       builder.UseAspNetCore()
                              .EnableTokenEndpointPassthrough()
                              .DisableTransportSecurityRequirement();

                       // develop case
                       builder.AddEphemeralSigningKey();
                       builder.AddEphemeralEncryptionKey();
                   })

                   // Register the OpenIddict validation components.
                   .AddValidation(options =>
                   {
                       // Import the configuration from the local OpenIddict server instance.
                       options.UseLocalServer();

                       // Register the ASP.NET Core host.
                       options.UseAspNetCore();
                   });

            Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
            // JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

            return service;
        }

        private static IServiceCollection AddJwtAuthorization(this IServiceCollection service)
        {
            service.AddAuthorization(config =>
            {
                config.DefaultPolicy = new AuthorizationPolicyBuilder(OpenIddictConstants.Schemes.Bearer)
                                       .RequireClaim(OpenIddictConstants.Claims.Subject)
                                       .Build();
            });

            return service;
        }

        private static IServiceCollection AddJwtAuthentication(this IServiceCollection service, BearerSettings settings)
        {
            service.AddAuthentication(options =>
                   {
                       options.DefaultAuthenticateScheme = OpenIddictConstants.Schemes.Bearer;
                       options.DefaultChallengeScheme = OpenIddictConstants.Schemes.Bearer;
                   })
                   .AddJwtBearer(options =>
                   {
                       options.SaveToken = true;

                       // develop case
                       options.RequireHttpsMetadata = false;
                       options.Authority = settings.Authority;

                       options.TokenValidationParameters = new TokenValidationParameters
                                                           {
                                                               ValidateAudience = false,
                                                               ValidateLifetime = true,

                                                               NameClaimType = OpenIddictConstants.Claims.Subject,
                                                               RoleClaimType = OpenIddictConstants.Claims.Role
                                                           };
                   });

            return service;
        }
    }
}