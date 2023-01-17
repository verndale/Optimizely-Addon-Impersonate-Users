using EPiServer.Authorization;
using EPiServer.Shell.Modules;
using EPiServer.Shell.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Verndale.ImpersonateUsers.Services;

namespace Verndale.ImpersonateUsers.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        private static readonly Action<AuthorizationPolicyBuilder> DefaultAuthorizationPolicyConfig = p => p.RequireRole(Roles.Administrators);

        public static IServiceCollection AddVerndaleImpersonation<TUser>(this IServiceCollection services) where TUser : IdentityUser, IUIUser, new()
            => services.AddVerndaleImpersonation<TUser>(DefaultAuthorizationPolicyConfig);

        public static IServiceCollection AddVerndaleImpersonation<TUser>(this IServiceCollection services, Action<AuthorizationPolicyBuilder> policyConfig) where TUser : IdentityUser, IUIUser, new()
            => services
                .AddScoped<IImpersonationService, ImpersonationService<TUser>>()
                .AddAuthorization(options => options.AddPolicy(Constants.AuthorizationPolicyName, policyConfig))
                .Configure<ProtectedModuleOptions>(
                    options =>
                    {
                        if (!options.Items.Any(module => module.Name.Equals(Constants.ModuleName, StringComparison.OrdinalIgnoreCase)))
                        {
                            options.Items.Add(new ModuleDetails { Name = Constants.ModuleName });
                        }
                    });
    }
}
