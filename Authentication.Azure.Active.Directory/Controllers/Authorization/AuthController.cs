namespace Authentication.Azure.Active.Directory.B2C.Controllers.Authorization
{
    using System.Threading.Tasks;
    using Authentication.Azure.Active.Directory.Infrastructure.Constants;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;

    [Route("auth")]
    public class AuthController : Controller
    {
        [Route("signup")]
        public IActionResult SignUp()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/" }, Policies.SignUp);
        }

        [Route("signin")]
        public IActionResult SignIn(string returnUrl)
        {
            var redirectUri = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;
            return Challenge(new AuthenticationProperties { RedirectUri = redirectUri }, Policies.SignIn);
        }

        [HttpPost]
        [Route("signout")]
        [ValidateAntiForgeryToken]
        public async Task SignOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                var scheme = User.FindFirst("tfp").Value;
                await HttpContext.SignOutAsync(scheme);
            }
        }

        [Route("profile")]
        public IActionResult Profile(string returnUrl)
        {
            var redirectUri = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;
            return Challenge(new AuthenticationProperties { RedirectUri = redirectUri }, Policies.Profile);
        }
    }
}