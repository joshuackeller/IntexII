using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using IntexII.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IntexII.Models;
using Microsoft.ML.OnnxRuntime;
namespace IntexII
{
    public class Startup
    {
        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var appUser = Environment.GetEnvironmentVariable("APP_USER");
            var appEmail = Environment.GetEnvironmentVariable("APP_EMAIL");
            var appPassword = Environment.GetEnvironmentVariable("APP_PASSWORD");
            var appAdminEmail = Environment.GetEnvironmentVariable("APP_ADMINEMAIL");
            //initializing custom roles 
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            string[] roleNames = { "Admin" };
            IdentityResult roleResult;
            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 1
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            //Here you could create a super user who will maintain the web app
            var poweruser = new IdentityUser
            {
                UserName = appUser,
                Email = appEmail,
                EmailConfirmed = true,
            };
            //Ensure you have these values in your appsettings.json file
            string userPWD = appPassword;
            var _user = await UserManager.FindByEmailAsync(appAdminEmail);
            if (_user == null)
            {
                var createPowerUser = await UserManager.CreateAsync(poweruser, userPWD);
                if (createPowerUser.Succeeded)
                {
                    //here we tie the new user to the role
                    await UserManager.AddToRoleAsync(poweruser, "Admin");
                }
            }
        }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var server = Environment.GetEnvironmentVariable("RDS_SERVER");
            var port = Environment.GetEnvironmentVariable("RDS_PORT");
            var user = Environment.GetEnvironmentVariable("RDS_USER");
            var password = Environment.GetEnvironmentVariable("RDS_PASSWORD");
            var database = Environment.GetEnvironmentVariable("RDS_DATABASE");
            var gooId = Environment.GetEnvironmentVariable("GOO_CLIENTID");
            var gooSecret = Environment.GetEnvironmentVariable("GOO_CLIENTSECRET");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql("server=" + server + ";port=" + port + ";database=" + database + ";uid=" + user + ";password=" + password));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddDbContext<RDSContext>(options =>
            {
                options.UseMySql("Data Source=" + server + ";Initial Catalog=" + database + ";User ID=" + user + ";Password=" + password + ";");
            });
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
            });
            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    //IConfigurationSection googleAuthNSection =
                    //Configuration.GetSection("Authentication:Google");
                    //options.ClientId = googleAuthNSection["ClientId"];
                    //options.ClientSecret = googleAuthNSection["ClientSecret"];
                    options.ClientId = gooId;
                    options.ClientSecret = gooSecret;
                });
            services.AddScoped<IRDSRepo, EFRDSRepo>();
            //services.AddScoped<ITeamsRepository, EFTeamsRepository>();
            services.AddSingleton<InferenceSession>(
                new InferenceSession("wwwroot/intex.onnx"));
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.Use(async (context, next) =>
                {
                    context.Response.Headers.Add("Strict-Transport-Security", "max-age-31536000");
                    context.Response.Headers.Add("X-Xss-Protection", "1");
                    context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
                    context.Response.Headers.Add("Feature-Policy",
                    "vibrate 'self' ; " +
                    "camera 'self' ; " +
                    "microphone 'self' ; " +
                    "speaker 'self'  ;  " +
                    "geolocation 'self' ; " +
                    "gyroscope 'self' ; " +
                    "magnetometer 'self' ; " +
                    "midi 'self' ; " +
                    "sync-xhr 'self' ; " +
                    "push 'self' ; " +
                    "notifications 'self' ; " +
                    "fullscreen '*' ; " +
                    "payment 'self' ; ");
                    await next();
                });
            }
            CreateRoles(serviceProvider).Wait();
            //Security response headers
            //
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            //CSP
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add(
                   "Content-Security-Policy-Report-Only",
                   "default-src 'self'; " +
                   "script-src-elem 'self'" +
                   "style-src-elem 'self'; " +
                   "img-src 'self'; " +
                   "font-src 'self'" +
                   "media-src 'self'" +
                   "frame-src 'self'" +
                   "connect-src "
                   );
                await next();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                     name: "filter",
                     pattern: "{severity}/{pageNum}",
                     defaults: new { controller = "Home", action = "Records" });
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "paging",
                    pattern: "{pageNum}",
                    defaults: new { controller = "Home", action = "Records", pageNum = 1 });
                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller=Home}/{action=Index}/{pageNum?}");
                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller=Home}/{action=SingleAction}/{id?}");
                //endpoints.MapControllerRoute(
                //    name: "admin",
                //    pattern: "{controller=Admin}/{action=Crashes}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}