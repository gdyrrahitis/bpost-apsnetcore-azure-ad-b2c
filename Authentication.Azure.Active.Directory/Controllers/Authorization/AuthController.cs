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

        [Route("login")]
        public IActionResult Login(string returnUrl)
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/" }, Policies.Login);
        }

        [Route("logout")]
        public IActionResult Logout(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public IActionResult Cancel() => RedirectToAction("Index", "Home");

        [HttpPost]
        [Route("logout")]
        [ValidateAntiForgeryToken]
        public async Task Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                var scheme = User.FindFirst("tfp").Value;
                await HttpContext.SignOutAsync(scheme);
            }
        }

        [Route("profile")]
        public IActionResult Profile()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/" }, Policies.Profile);
        }
    }
}