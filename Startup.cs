using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ReactDotNetBoiler
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication(o =>
                {
                    o.DefaultScheme = "Application";
                    o.DefaultSignInScheme = "External";
                })
                .AddCookie("Application")
                .AddCookie("External")
                .AddGoogle(o =>
                {
                    o.ClientId = Configuration["Google:ClientId"]; 
                    o.ClientSecret = Configuration["Google:ClientSecret"]; 
                    o.UserInformationEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";
                    o.ClaimActions.Clear();
                    o.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                    o.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                    o.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
                    o.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
                    o.ClaimActions.MapJsonKey("urn:google:profile", "link");
                    o.ClaimActions.MapJsonKey("image", "picture");
                    o.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();

            app.UseCors(policy => policy.SetIsOriginAllowed(origin => origin == "https://accounts.google.com/"));
            app.UseAuthentication();
            app.Use(async (context, next) =>
            {
                if (!context.User.Identity.IsAuthenticated && context.Request.Path != "/signin-google" && context.Request.Path != "/Account/Login" & context.Request.Path != "/Account/LoginCallBack")
                {
                    await context.ChallengeAsync("External");
                }
                else
                {
                    await next();
                }
            });

            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            
            app.UseMvc(routes => routes.MapRoute("default", "{controller}/{action=Index}/{id?}"));

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
           
        }
    }
}
