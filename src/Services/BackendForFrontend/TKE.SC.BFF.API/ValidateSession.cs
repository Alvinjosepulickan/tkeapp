using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using TKE.SC.BFF.Helper;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.ExceptionModel;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;

namespace TKE.SC.BFF
{
    [ExcludeFromCodeCoverage]
    public class ValidateSessionAttribute : Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute
    {
        /// <summary>
        /// Extracting out the JWT payload data and check for Session Id validity.
        /// </summary>
        /// <param Name="filterContext"></param>
        public override async void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var assembly = typeof(ValidateSessionAttribute).Assembly;
                if (context != null)
                {
                    var distributedCache =
                        (IDistributedCache)context.HttpContext.RequestServices.GetService(typeof(IDistributedCache));
                    var cacheManager =
                       (ICacheManager)context.HttpContext.RequestServices.GetService(typeof(ICacheManager));
                    var iConfiguration =
                        (IConfiguration)context.HttpContext.RequestServices.GetService(typeof(IConfiguration));
                    var ihttpService =
                        (IHttpClientFactory)context.HttpContext.RequestServices.GetService(typeof(IHttpClientFactory));
                    //var iLoggerService = (ILoggerFactory)context.HttpContext.RequestServices.GetService(typeof(ILoggerFactory));
                    // var accessBlLogger = (ILogger<AccessBL>)(context.HttpContext.RequestServices.GetService(typeof(ILogger<AccessBL>)));
                    var environment = iConfiguration.GetSection(Constant.PARAMSETTINGS)[Constant.ENVIRONMENT];
                    //var access = new AccessBL(iConfiguration, new AccessTokenDL(iConfiguration, new CpqCacheManager(distributedCache), new HttpContextAccessor(), distributedCache, ihttpService),
                    //    new CpqCacheManager(distributedCache), new HttpContextAccessor(), distributedCache);
                    //var access = new AccessBL(iConfiguration, cacheManager, new HttpContextAccessor(), accessBlLogger);
                    var cpqCacheManager = cacheManager;
                    //Extracting the embedded data from Header
                    var sessionId = context.HttpContext.Request.Headers[Constant.SESSIONID];
                    var locale = context.HttpContext.Request.Headers[Constant.LOCALEWORD];
                    if (string.IsNullOrEmpty(sessionId))
                    {
                        var errorBody = new
                        {
                            StatusCode = Constant.UNAUTHORIZED,
                            Message = Constant.SESSIONIDMISSINGMESSAGE

                        };
                        context.Result = new ObjectResult(errorBody)
                        {
                            StatusCode = 401
                        };
                        return;
                    }
                    var status = cpqCacheManager.GetCache(sessionId, environment, Constant.CPQ);
                    var tkeUuid =
                        cpqCacheManager.GetCache(sessionId, environment, Constant.TKEUUID);
                    context.HttpContext.Items[Constant.STATUS] = status;
                    if (status == null)
                    {
                        var errorBody = new
                        {
                            StatusCode = Constant.UNAUTHORIZED,
                            Message = Constant.SESSIONEXPIRYMESSAGE

                        };
                        context.Result = new ObjectResult(errorBody)
                        {
                            StatusCode = 401
                        };

                        return;
                    }
                    //Locale validation
                    var cachedLocale = cpqCacheManager.GetCache(sessionId, environment, Constant.LOCALEWORD);
                    if (!string.IsNullOrEmpty(context.HttpContext.Request.Headers[Constant.LOCALEWORD]))
                    {
                        if (!(cachedLocale.ToLower().Equals(context.HttpContext.Request.Headers[Constant.LOCALEWORD].ToString().ToLower())))
                        {
                            var errorBody = new
                            {
                                StatusCode = (int)HttpStatusCode.PreconditionFailed,
                                Message = Constant.LOCALEERROR

                            };
                            context.Result = new ObjectResult(errorBody)
                            {
                                StatusCode = (int)HttpStatusCode.PreconditionFailed,
                            };

                            return;
                        }
                    }
                    var uid = cpqCacheManager.GetCache(sessionId, environment, Constant.UID);
                    context.HttpContext.Items[Constant.SESSIONID] = sessionId;


                    context.HttpContext.Items[Constant.LOCALEWORD] = locale;
                    var queryString = context.HttpContext.Request.QueryString;
                    var requestBody = context.HttpContext.Request.Body;
                    context.HttpContext.Items[Constant.UID] = uid;
                    context.HttpContext.Items["queryString"] = queryString;
                    if (requestBody.CanSeek)
                    {
                        context.HttpContext.Items["requestBody"] = requestBody.Length;
                    }
                    base.OnActionExecuting(context);
                }
            }
            catch (Exception ex)
            {

                // .LogError("Internal error in Validate session: " + ex.ToString());

                var errorBody = new
                {
                    StatusCode = Constant.UNAUTHORIZED,
                    Message = Constant.INTERNALSERVERERROR

                };
                if (context != null)
                {
                    context.Result = new ObjectResult(errorBody)
                    {
                        StatusCode = 500
                    };
                }
                return;
            }

        }
    }
}
