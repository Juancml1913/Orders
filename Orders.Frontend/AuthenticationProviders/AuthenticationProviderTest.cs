using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Orders.Frontend.AuthenticationProviders
{
    public class AuthenticationProviderTest : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            await Task.Delay(500); // Simulate async operation
            var anonymous = new ClaimsIdentity();
            var user = new ClaimsIdentity(authenticationType: "test");
            var admin = new ClaimsIdentity(new List<Claim> {
                new Claim("FirstName", "Juan"),
                new Claim("LastName", "Muñoz"),
                new Claim(ClaimTypes.Name, "munoz@yopmail.com"),
                new Claim(ClaimTypes.Role, "Admin"),
            },
            authenticationType: "test");
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(anonymous)));
        }
    }
}
