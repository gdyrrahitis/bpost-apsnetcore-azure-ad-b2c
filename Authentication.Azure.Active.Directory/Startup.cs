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
    using System;
    using System.Threading.Tasks;

    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = Policies.SignUpLogin;
            })
            .AddOpenIdConnect(Policies.SignUp, options => SetupOpenIdConnectOptions(options, Policies.SignUp))
            .AddOpenIdConnect(Policies.Login, options => SetupOpenIdConnectOptions(options, Policies.Login))
            .AddOpenIdConnect(Policies.SignUpLogin, options => SetupOpenIdConnectOptions(options, Policies.SignUpLogin))
            .AddOpenIdConnect(Policies.Profile, options =>
            {
                options.Events = new OpenIdConnectEvents
                {
                    OnRemoteFailure = context =>
                    {
                        context.Response.Redirect("/");
                        context.HandleResponse();
                        return Task.CompletedTask;
                    }
                };
                SetupOpenIdConnectOptions(options, Policies.Profile);
            })
            .AddCookie();
        }

        private void SetupOpenIdConnectOptions(OpenIdConnectOptions options, string policy)
        {
            options.MetadataAddress =  $"https://login.microsoftonline.com/TENANT_ID.onmicrosoft.com/v2.0/.well-known/openid-configuration?p={policy}";
            options.ClientId = "CLIENT_ID";
            options.ResponseType = OpenIdConnectResponseType.IdToken;
            options.CallbackPath = $"/login/{policy}";
            options.SignedOutCallbackPath = $"/logout/{policy}";
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
