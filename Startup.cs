using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhuKienDienThoai.Data;
using PhuKienDienThoai.Models;
using PhuKienDienThoai.Services;
using PhuKienDienThoai.Configurations;

namespace PhuKienDienThoai
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("_appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"_appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddSession();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(opt =>
            {
                opt.SignIn.RequireConfirmedEmail = false;
                opt.User.RequireUniqueEmail = true;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireNonAlphanumeric = false;
            });
            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddMvc()
                    .AddSessionStateTempDataProvider();

            // services.AddAuthentication().AddFacebook(facebookOptions =>
            // {
            //     facebookOptions.AppId = "1585151324855686";
            //     facebookOptions.AppSecret = "0cebe3614d74d124ddd698867645a22a";
            // });        
            services.AddSingleton<IPaypalServices, PaypalServices>();
            services.Configure<PayPalAuthOptions>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseDeveloperExceptionPage();
            app.UseBrowserLink();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                name: "payment",
                template: "{controller=Payment}/{action=Index}/{id?}");
            });
            // try
            // {
            //     var roleseeder = new PhuKienDienThoai.Configurations.RoleSeeder(serviceProvider);
            //     roleseeder.SeedAsync().Wait();

            // }
            // catch (System.Exception ex)
            // {

            //     System.Console.WriteLine(ex.ToString());
            // }

        }
    }
}
