namespace Authentication.Azure.Active.Directory.B2C.Controllers.Home
{
    using System.Collections.Generic;
    using System.Security.Claims;

    public class ProfileViewModel
    {
        public string Name { get; set; }
        public IEnumerable<Claim> Claims { get; set; }
    }
}