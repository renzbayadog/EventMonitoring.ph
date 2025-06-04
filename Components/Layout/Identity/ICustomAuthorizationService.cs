using System.Security.Claims;

namespace EventMonitoring.Components.Layout.Identity
{
    public interface ICustomAuthorizationService
    {
        bool CustomClaimChecker(ClaimsPrincipal user, string specificClaim);
    }
}
