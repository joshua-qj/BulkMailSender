using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BulkMailSender.Blazor.Services {
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider {
        public override Task<AuthenticationState> GetAuthenticationStateAsync() {
            // Define claims for an anonymous user with specific permissions
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, "dc5809a6-b4a5-47b7-82f3-5735acc14a4b"),
        new Claim(ClaimTypes.Name, "Anonymous"),
        new Claim("Permission", "Admin"),
        new Claim("Permission", "CanAccessEmailSending")
    };

            // Create an identity with a specified authentication type to mark it as authenticated
            var identity = new ClaimsIdentity(claims, authenticationType: "MockAuthentication");

            // Create a ClaimsPrincipal with the identity
            var user = new ClaimsPrincipal(identity);

            // Return the authentication state
            return Task.FromResult(new AuthenticationState(user));
        }
    }
}
