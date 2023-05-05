using TKE.CPQ.IdentityServer.Data;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using TKE.CPQ.IdentityServer.Extensions;

namespace TKE.CPQ.IdentityServer
{
    public class Startup
    {
        public IConfiguration _configuration { get; }
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(config => {
                config.UseInMemoryDatabase("MemoryDB");
            });

            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            services.AddIdentityServer()
                .AddInMemoryClients(_configuration.GetSection("IdentityServer:Clients"))
                .AddInMemoryIdentityResources(_configuration.GetSection("IdentityServer:IdentityResources"))
                .AddInMemoryApiResources(_configuration.GetSection("IdentityServer:ApiResources"))
                .AddInMemoryApiScopes(_configuration.GetSection("IdentityServer:ApiScopes"))
                .AddAspNetIdentity<AppUser>()
                .AddProfileService<ProfileService>()
                .AddDeveloperSigningCredential();

            services.AddControllersWithViews();

            services.AddAuthentication()
                .AddOpenIdSchemes(_configuration.GetSection("IdentityServer:ExternalProviders"));

            // To seed local users on startup
            if(Convert.ToBoolean(_configuration.GetSection("IdentityServer:SeedLocalUsers").Value.ToString()))
			{
                SeedUsers.EnsureSeedData(_configuration.GetSection("IdentityServer:Users"));
            }

            services.AddTransient<IGraphQuery, GraphQuery>();
            //services.AddTransient<IProfileService, ProfileService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();


            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }
    }
}
