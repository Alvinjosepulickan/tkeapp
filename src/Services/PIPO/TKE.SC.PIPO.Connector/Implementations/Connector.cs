using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Description;
using System.Threading.Tasks;
using SpecMemoService;
using System.ServiceModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static TKE.SC.PIPO.Constants;

namespace TKE.SC.PIPO
{
    public sealed class Connector : IPipoConnector
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Connector> _logger;
        private readonly IServiceConfiguration _serviceConfiguration;

        public Connector(IConfiguration configuration, ILogger<Connector> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _serviceConfiguration = new ServiceConfiguration(_configuration);

        }

        private SpecMemoOBOM_CreateUpdate_OutClient ConfigureSpecMemoClient(string pipoSettings)
        {
            var bindings = _serviceConfiguration.GetBindings();
            var endPoint = _serviceConfiguration.GetEndpointAddress(pipoSettings);

            var _specMemoClient = new SpecMemoOBOM_CreateUpdate_OutClient(bindings, endPoint);

            return _serviceConfiguration.ConfigureCredentials(_specMemoClient, pipoSettings);
        }

        public async Task<bool> CreateOrUpdateSpecMemoAsync(string specMemoRequestXml, ContentType contentType= ContentType.FilePath, string pipoSettings = "PIPOSettings")
        {
            var client = ConfigureSpecMemoClient(pipoSettings);
            var deserializer = new XmlHelper();
            var request = contentType == ContentType.FilePath ? 
                                            deserializer.DeserializeXml(specMemoRequestXml) : 
                                            deserializer.DeserializeString(specMemoRequestXml);

            try
            {
                await client.SpecMemoOBOM_CreateUpdate_OutAsync(request);
            }
            catch (ProtocolException ex)
            {
                _logger?.LogInformation(ex.Message);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.Message);
                return false;
            }
            return true;
        }
    }
}
