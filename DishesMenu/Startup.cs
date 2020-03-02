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
using DishesMenu.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using DishesMenu.Services;
using DishesMenu.Utility;
using Stripe;

/*
    92. pogledao vezano uz Extensions, ali bi trebao još jednom
    106. pregledo samo vratiti na otprilike 2 min da zapisem sto je IWebHostEnvironment
    180. pogledaj za stripe 

    section9: landing index page
    section10: adding registration roles


    NISAM POGLEDAO 166.
*/

namespace DishesMenu
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            // options => options.SignIn.RequireConfirmedAccount = true
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();

            services.AddSingleton<IEmailSender, EmailSender>();

            // Stripe
            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));

            /*
                .AddRazorRuntimeCompilation()
                if w make any change in the views when app is running this will
                automatically refreshed when refresh button is clicked 
            */
            services.AddRazorPages().AddRazorRuntimeCompilation();

            // configure session on web site
            services.AddSession(options =>
            {
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // for DotNet Core 3.0 
            StripeConfiguration.ApiKey = Configuration.GetSection("Section")["SecretKey"];

            //for DotNet Core 2.2 StripeConfiguration.SetApiKey(Configuration.GetSection("Section")["SecretKey"]);

            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
