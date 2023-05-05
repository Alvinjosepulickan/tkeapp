﻿using Microsoft.AspNetCore.Identity;

namespace TKE.CPQ.IdentityServer.Data
{
	public class AppUser : IdentityUser
    {
        // Add additional profile data for application users by adding properties to this class
        public string Name { get; set; }
    }
}
