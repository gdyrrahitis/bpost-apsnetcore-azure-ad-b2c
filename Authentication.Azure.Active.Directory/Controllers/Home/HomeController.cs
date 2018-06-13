namespace Authentication.Azure.Active.Directory.B2C.Controllers.Home
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        [Authorize]
        [Route("profile")]
        public IActionResult Profile()
        {
            var model = new ProfileViewModel
            {
                Name = User.Identity.Name,
                Claims = User.Claims
            };
            return View(model);
        }
    }
}
