using codegen.Extensions.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace EventMonitoring.Components.Layout.Identity
{
    public static class SignOutEndPoint
    {   
        public static IEndpointConventionBuilder MapSignOutEndpoint(this IEndpointRouteBuilder endpoint)
        {
            ArgumentNullException.ThrowIfNull(endpoint);
            var accountGroup = endpoint.MapGroup("/Account");

            accountGroup.MapPost("/Logout", async (ClaimsPrincipal user, SignInManager<User> signInManager) =>
            {
                await signInManager.SignOutAsync(); //clear token in cookie
                return TypedResults.LocalRedirect("/Account/Login");
            });

            return accountGroup;
        }
    }
}
