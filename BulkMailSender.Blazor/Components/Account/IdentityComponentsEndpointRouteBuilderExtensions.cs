using BulkMailSender.Application.UseCases.Identity.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.AspNetCore.Routing {
    internal static class IdentityComponentsEndpointRouteBuilderExtensions
    {
        // These endpoints are required by the Identity Razor components defined in the /Components/Account/Pages directory of this project.
        public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
        {
            ArgumentNullException.ThrowIfNull(endpoints);

            var accountGroup = endpoints.MapGroup("/Account");

            accountGroup.MapPost("/Logout", async (
                //ClaimsPrincipal user,

                 [FromServices] ILogoutUseCase logoutUseCase,
                [FromForm] string returnUrl) =>
            {
                await logoutUseCase.ExecuteAsync();
                return TypedResults.LocalRedirect($"~/{returnUrl}");
            });
            return accountGroup;
       
        }
    }
}
