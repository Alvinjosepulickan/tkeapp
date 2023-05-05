using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.Helper;
using TKE.SC.Common.Model.RequestModel;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model.ViewModel;

namespace TKE.SC.BFF.Api.V1
{

    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/v{versoin:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        /// Variables Collection
        /// Controller Created
        /// </summary>
        #region Variables
        private readonly ILogger _logger;
        private readonly IAuth _auth;
        private readonly IGenerateToken _generateToken;
        private readonly IConfigure _configure;
        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        /// <summary>
        /// Constructor for AuthController
        /// </summary>
        /// <param Name="auth"></param>
        /// <param Name="generateToken"></param>
        /// <param Name="httpContextAccessor"></param>
        /// <param Name="logger"></param>

        public AuthController(IAuth auth, IGenerateToken generateToken, IHttpContextAccessor httpContextAccessor, ILogger<AuthController> logger)
        {
            _auth = auth;
            _generateToken = generateToken;
            _httpContextAccessor = httpContextAccessor;
            Utility.SetLogger(logger);
        }

        /// <summary>
        /// Get logged in user details .
        /// </summary>
        /// <param Name="userId"></param>
        /// <returns></returns>
        [Route("currentuser")]
        [HttpGet]
        public async Task<IActionResult> GetUserDetails()
        {

            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            //create a user model
            User userDetailsModel = new User
            {
                UserId = Utility.GetClaim(User, JwtClaimTypes.PreferredUserName),
                Groups = Utility.GetClaims(User, Constant.GROUPS),
                FirstName = Utility.GetClaim(User, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"),
                LastName = Utility.GetClaim(User, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname"),
                Email = Utility.GetClaim(User, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")
            };
            //for testing SM role
            //TODO: Remove asap after Testing team confirmation for SM role 
            if (userDetailsModel.Email.Equals("sree.kona@tkelevator.com", StringComparison.OrdinalIgnoreCase) && userDetailsModel.Groups.Contains("be90f066-a2c8-4822-8062-d7276d671cf5"))
            {
                userDetailsModel.Groups.Remove("be90f066-a2c8-4822-8062-d7276d671cf5");
                userDetailsModel.Groups.Add("58dc206c-3e53-49bb-bb60-2727966928c2");
            }
            var UserRespObj = await _auth.GetUserDetails(userDetailsModel, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(UserRespObj);
        }
     
    }

}

