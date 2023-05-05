using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.ExceptionModel;

namespace TKE.SC.BFF
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private static ILogger _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            this.next = next;
            _logger = loggerFactory.CreateLogger<ExceptionHandlingMiddleware>();
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (CustomException ex)
            {
                await HandleExceptionAsync(context, ex);
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.Flatten().InnerExceptions)
                {
                    await HandleExceptionAsync(context, e);
                }
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync<T>(HttpContext context, T exception)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected
            var result = new ResponseMessage();
            var logMessage = string.Empty;
            var defaultException = (Exception)(object)exception;

            if (exception is InvalidOperationException)
            {
                result = new ResponseMessage
                {
                    Message = Helper.Constant.INVALIDEXCEPTIONLOGGING,
                    StatusCode = (int)HttpStatusCode.Forbidden
                };
                logMessage = $"{Helper.Constant.SOMETHINGWENTWRONG} {defaultException.ToString()} {Helper.Constant.CLOSINGBRACKET}";
            }
            else if (defaultException is CustomException customException)
            {

                if (customException.Code == Constant.INTERNALSERVERERROR.ToString())
                {
                    result = new ResponseMessage
                    {
                        Message = string.IsNullOrEmpty(customException.ExceptionMessage) ? $"{Helper.Constant.SOMETHINGWENTWRONG}" : customException.ExceptionMessage,
                        Description = string.IsNullOrEmpty(customException.ExceptionMessage) ? $"{Helper.Constant.SOMETHINGWENTWRONG}" : customException.ExceptionMessage,
                        StatusCode = int.Parse(customException.Code)
                    };
                }
                else
                {
                    result = new ResponseMessage
                    {
                        Message = $"{customException.ExceptionMessage}",
                        StatusCode = int.Parse(customException.Code),
                        ResponseArray = customException.ResponseArray
                    };
                    
                }
                Enum.TryParse(customException.Code, out code);
                logMessage = $"{customException.ExceptionMessage}";
            }

            else if (defaultException is ExternalCallException externalCallException)
            {

                if (externalCallException?.HttpResponseMessage?.StatusCode == HttpStatusCode.InternalServerError)
                {
                    result = new ResponseMessage
                    {
                        Message = $"{externalCallException.ExceptionMessage}",
                        StatusCode = (int)externalCallException.HttpResponseMessage.StatusCode
                    };

                }
                else if (externalCallException?.HttpResponseMessage?.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    result = new ResponseMessage
                    {
                        Description = externalCallException.Description,
                        Message = Helper.Constant.SERVICEUNAVAILABLEEXCEPTION,
                        StatusCode = (int)externalCallException.HttpResponseMessage.StatusCode
                    };
                }
                else
                {
                    result = new ResponseMessage
                    {
                        Description = externalCallException.Description,
                        Message = $"{externalCallException.ExceptionMessage}",
                        StatusCode = 400
                    };
                }
                Enum.TryParse(400.ToString(), out code);
                logMessage = $"{Helper.Constant.ERRORWHILECONNECTINGEXTERNALAPI},{externalCallException.ExceptionMessage}";
            }

            else
            {

                result = new ResponseMessage
                {
                    Message = $"{Helper.Constant.SOMETHINGWENTWRONG}",
                    StatusCode = (int)code
                };
                if (result.Message.Contains(Constant.NOAUTHENTICATIONSCHEME))
                {
                    result.StatusCode = Constant.FORBIDDEN;
                    result.Message = Constant.INVALIDEXCEPTIONLOGGING;
                    result.Description = Constant.SOMETHINGWENTWRONG;
                }
                logMessage = $"{defaultException.Message}";
            }
            result.ReferenceId = context.TraceIdentifier;
            _logger.LogError(defaultException, logMessage);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = result.StatusCode;
            var response = Utility.SerializeObjectValue(result);
            return context.Response.WriteAsync(response);
        }
    }
}
