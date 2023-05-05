using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TKE.CPQ.IdentityServer.Data
{
    public class ProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<AppUser> _claimsFactory;
        private readonly UserManager<AppUser> _userManager;

        public ProfileService(UserManager<AppUser> userManager, IUserClaimsPrincipalFactory<AppUser> claimsFactory)
        {
            _userManager = userManager;
            _claimsFactory = claimsFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.Claims.Where(x => x.Type == "sub").FirstOrDefault().Value;
            var sessionId = context.Subject.Claims.Where(x => x.Type == "SessionId").FirstOrDefault().Value;
            var user = await _userManager.FindByIdAsync(sub);
            var principal = await _claimsFactory.CreateAsync(user);

            var claims = principal.Claims.ToList();
            //claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();
            if (sessionId.Length > 0)
            {
                claims.Add(new Claim("SessionId", sessionId));
            }

            claims.Add(new Claim(JwtClaimTypes.GivenName, user.Name));
            claims.Add(new Claim(IdentityServerConstants.StandardScopes.Email, user.Email));
        
            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.Claims.Where(x => x.Type == "sub").FirstOrDefault().Value;
            var user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }

    }
}
