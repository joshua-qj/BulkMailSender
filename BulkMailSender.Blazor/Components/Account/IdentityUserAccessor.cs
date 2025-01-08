
using Microsoft.AspNetCore.Identity;
// this class is depricated.
//namespace BulkMailSender.Blazor.Components.Account
//{
//    internal sealed class IdentityUserAccessor(UserManager<ApplicationUser> userManager, IdentityRedirectManager redirectManager)
//    {
//        public async Task<ApplicationUser> GetRequiredUserAsync(HttpContext context)
//        {
//            var user = await userManager.GetUserAsync(context.User);

//            if (user is null)
//            {
//                redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
//            }

//            return user;
//        }
//    }
//}
