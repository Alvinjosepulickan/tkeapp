using Microsoft.Extensions.Logging;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using TKE.SC.BFF.DataAccess.Helpers;

namespace TKE.SC.BFF.DataAccess.Services
{
    public class WsSecurityEndpointBehavior : IEndpointBehavior
    {
        private readonly string _password;
        private readonly string _username;
        private readonly ILogger<WsSecurityEndpointBehavior> _logger;

        public WsSecurityEndpointBehavior(string username, string password, ILogger<WsSecurityEndpointBehavior> logger)
        {
            _username = username;
            _password = password;
            Utility.SetLogger(logger);
        }

        void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint,
            BindingParameterCollection bindingParameters)
        {
        }

        void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            var methodBegin = Utility.LogBegin();
            ILogger<WsSecurityMessageInspector> loggers = (ILogger<WsSecurityMessageInspector>) _logger;
            clientRuntime.ClientMessageInspectors.Add(new WsSecurityMessageInspector(_username, _password, loggers));
            Utility.LogEnd(methodBegin);
        }
        void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        void IEndpointBehavior.Validate(ServiceEndpoint endpoint)
        {
        }
    }
}
