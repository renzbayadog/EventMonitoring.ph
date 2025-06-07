using codegen.Extensions.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace EventMonitoring.Components.Layout.Identity
{
    internal class IdentityRevalidatingAuthStateProvider(ILoggerFactory loggerfactory,
                                                         IServiceScopeFactory scopeFactory,
                                                         IOptions<IdentityOptions> options): 
                                                         RevalidatingServerAuthenticationStateProvider(loggerfactory)
    {
        protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(5);

        protected async override Task<bool> ValidateAuthenticationStateAsync(
            AuthenticationState authenticationState, CancellationToken cancellationToken)
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            return await ValidateSecurityStampAsync(userManager, authenticationState.User);

        }
        public async Task<bool> ValidateSecurityStampAsync(UserManager<User> userManager, ClaimsPrincipal principal)
        {
            var user = await userManager.GetUserAsync(principal);

            if (user is null)
            {
                return false;
            }
            else if (!userManager.SupportsUserSecurityStamp)
            {
                return true;
            }
            else
            {
                var principalStamp = principal.FindFirstValue(options.Value.ClaimsIdentity.SecurityStampClaimType);
                var userStamp = await userManager.GetSecurityStampAsync(user);
                return principalStamp == userStamp;
            }
        }
    }
}
