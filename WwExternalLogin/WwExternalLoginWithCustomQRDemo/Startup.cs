using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WwExternalLoginWithCustomQRDemo.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WwExternalLoginWithCustomQRDemo
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
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages();

            var wwOpts = Configuration.GetSection("WwOptions").Get<WwOptions>();

            // ��� ���� ��½
            services.AddAuthentication().AddWw(opts =>
            {
                opts.ClientId = wwOpts.ClientId;
                opts.ClientSecret = wwOpts.ClientSecret;

                opts.AgentId = wwOpts.AgentId;

                opts.SignInScheme = IdentityConstants.ExternalScheme;

                opts.SignInScheme = IdentityConstants.ExternalScheme;

                // ������ʹ���Լ��� ɨ�� ҳ�棬����붨���Լ��� ��Ȩ�ڵ�
                opts.AuthorizationEndpoint = "/Identity/Account/WwLogin";

                opts.Events.OnRemoteFailure = async ctx =>
                {
                    var tempDataProvider = ctx.HttpContext.RequestServices.GetRequiredService<ITempDataProvider>();

                    tempDataProvider.SaveTempData(ctx.HttpContext, new Dictionary<string, object>
                            {
                        { "ErrorMessage",ctx.Failure.Message }
                            });
                    ctx.Response.Redirect("/Identity/Account/Login");
                    ctx.HandleResponse();

                    await Task.CompletedTask;
                };
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
