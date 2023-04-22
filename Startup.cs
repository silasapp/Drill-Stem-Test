using System;
using DST.Helpers;
using DST.Models.DB;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rotativa.AspNetCore;

namespace DST
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

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.ConfigureApplicationCookie(options => options.LoginPath = "/Account/AccessDenied");

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDbContext<DST_DBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DSTConnectionString")));

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);//You can set Time   
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddMvc().AddNewtonsoftJson();

            ElpsServices._elpsAppEmail = Configuration.GetSection("ElpsKeys").GetSection("elpsAppEmail").Value.ToString();
            ElpsServices._elpsBaseUrl = Configuration.GetSection("ElpsKeys").GetSection("elpsBaseUrl").Value.ToString();
            ElpsServices.public_key = Configuration.GetSection("ElpsKeys").GetSection("PK").Value.ToString();
            ElpsServices._elpsAppKey = Configuration.GetSection("ElpsKeys").GetSection("elpsSecretKey").Value.ToString();
            ElpsServices.link = Configuration.GetSection("NominationLink").GetSection("Link").Value.ToString();

           
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Account/AccessDenied");
                });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [Obsolete]
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
                app.UseDeveloperExceptionPage();
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSession();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}/{option?}");
            });

            RotativaConfiguration.Setup(env);
        }
    }
}
