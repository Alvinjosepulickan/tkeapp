using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace TKE.CPQ.IdentityServer.Data
{
    public class SeedUsers
    {
        public class LocalUser : AppUser
        {
            public string RoleGuid { get; set; }
            public string BranchGuid { get; set; }

        }
        public static void EnsureSeedData(IConfigurationSection usersSection)
        {
            var services = new ServiceCollection();
			services.AddLogging();
			services.AddDbContext<ApplicationDbContext>(config =>
			{
				config.UseInMemoryDatabase("MemoryDB");
			});

			services.AddIdentity<AppUser, IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    //context.Database.Migrate();

                    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                    List<LocalUser> users = usersSection.Get<List<LocalUser>>();

                    foreach (var user in users)
                    {
                        var currentUser = userMgr.FindByNameAsync(user.Email).Result;
                        if (currentUser == null)
                        {
                            currentUser = new AppUser
                            {
                                UserName = user.Email,
                                Email = user.Email,
                                Name = user.Name,
                                EmailConfirmed = true
                            };


                            var result = userMgr.CreateAsync(currentUser, "C2d@1234").Result;
                            if (!result.Succeeded)
                            {
                                throw new Exception(result.Errors.First().Description);
                            }

                            result = userMgr.AddClaimsAsync(currentUser, AddADClaims(user) ).Result;
                            if (!result.Succeeded)
                            {
                                throw new Exception(result.Errors.First().Description);
                            }
                            Claim[] AddADClaims(LocalUser user)
                            {
                                return new Claim[6]{
                                new Claim("groups", user.RoleGuid),
                                new Claim("groups", user.BranchGuid),
                                new Claim(JwtClaimTypes.PreferredUserName, user.Email ),
                                new Claim(JwtClaimTypes.GivenName, user.Name.Split(" " )[0]),
                                new Claim(JwtClaimTypes.FamilyName, user.Name.Split(" " )[1]),
                                new Claim(JwtClaimTypes.Email, user.Email )
                                };
                            }
                            }
                    }
                }
            }
        }
    }
}