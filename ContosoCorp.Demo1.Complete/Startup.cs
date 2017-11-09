using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ContosoCorp.Demo1.Complete.Data;
using ContosoCorp.Demo1.Complete.Models;
using ContosoCorp.Demo1.Complete.Services;

namespace ContosoCorp.Demo1.Complete
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
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("EmployeeNumber"));

                opt.AddPolicy("FoundersOnly", policy => policy.RequireClaim("EmployeeNumber", "1", "2", "3", "4", "5"));

                opt.AddPolicy("Over21", policy => policy.RequireAssertion(context =>
                {
                    if (context.User.HasClaim(c => c.Type == "DateOfBirth"))
                    {
                        var dob = DateTime.Parse(context.User.FindFirst("DateOfBirth").Value);
                        var bornOnOrBefore = DateTime.Today.AddYears(-21);

                        return dob <= bornOnOrBefore;
                    }
                    else
                    {
                        return false;
                    }
                }));
            });

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

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
