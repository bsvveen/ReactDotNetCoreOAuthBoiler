using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public IActionResult Login(string returnUrl) =>
            new ChallengeResult(
                GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action(nameof(LoginCallback), new { returnUrl })
                });

        [AllowAnonymous]
        public async Task<IActionResult> LoginCallback(string returnUrl)
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("External");

            if (!authenticateResult.Succeeded)
                return BadRequest();

            var claimsIdentity = new ClaimsIdentity("Application");

            claimsIdentity.AddClaim(authenticateResult.Principal.FindFirst(ClaimTypes.Name));
            claimsIdentity.AddClaim(authenticateResult.Principal.FindFirst(ClaimTypes.Email));
            claimsIdentity.AddClaim(authenticateResult.Principal.FindFirst("image"));

            await HttpContext.SignInAsync(
                "Application",
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties { IsPersistent = true }); // IsPersistent will set a cookie that lasts for two weeks (by default).

            await HttpContext.SignOutAsync("External");

            return LocalRedirect(returnUrl??"/");
        }

        [Authorize]
        public async Task<IActionResult> GetUserInfo()
        {
            var user = new UserInfo(HttpContext.User);
            return Ok(user);
        }

        private class UserInfo
        {
            private readonly ClaimsPrincipal _user;
            public UserInfo(ClaimsPrincipal user) => _user = user;

            public string Name => _user.FindFirst(ClaimTypes.Name).Value;
            public string Email => _user.FindFirst(ClaimTypes.Email).Value;
            public string Image => _user.FindFirst("image").Value;
        }
    }
}