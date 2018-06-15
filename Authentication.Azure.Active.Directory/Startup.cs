namespace Authentication.Azure.Active.Directory.B2C
{
    using Authentication.Azure.Active.Directory.Infrastructure.Constants;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication.OpenIdConnect;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Rewrite;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;
    using System.Threading.Tasks;

    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            var events = new OpenIdConnectEvents
            {
                OnRemoteFailure = context =>
                {
                    // You might want to log the error
                    context.Response.Redirect("/");
                    context.HandleResponse();
                    return Task.CompletedTask;
                }
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = Policies.SignUpIn;
            })
            .AddOpenIdConnect(Policies.SignUp, options =>
            {
                options.Events = events;
                SetupOpenIdConnectOptions(options, Policies.SignUp);
            })
            .AddOpenIdConnect(Policies.SignIn, options => SetupOpenIdConnectOptions(options, Policies.SignIn))
            .AddOpenIdConnect(Policies.SignUpIn, options =>
            {
                options.Events = events;
                SetupOpenIdConnectOptions(options, Policies.SignUpIn);
            })
            .AddOpenIdConnect(Policies.Profile, options =>
            {
                options.Events = events;
                SetupOpenIdConnectOptions(options, Policies.Profile);
            })
            .AddCookie();
        }

        private void SetupOpenIdConnectOptions(OpenIdConnectOptions options, string policy)
        {
            options.MetadataAddress = $"https://login.microsoftonline.com/<TENANT>.onmicrosoft.com/v2.0/.well-known/openid-configuration?p={policy}";
            options.ClientId = "<CLIENT_ID>";
            options.ResponseType = OpenIdConnectResponseType.IdToken;
            options.CallbackPath = $"/signin/{policy}";
            options.SignedOutCallbackPath = $"/signout/{policy}";
            options.SignedOutRedirectUri = "/";
            options.TokenValidationParameters.NameClaimType = "name";
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseBrowserLink();
            app.UseDeveloperExceptionPage();

            var options = new RewriteOptions()
                .AddRedirectToHttps(StatusCodes.Status301MovedPermanently, 44379);
            app.UseRewriter(options);
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
