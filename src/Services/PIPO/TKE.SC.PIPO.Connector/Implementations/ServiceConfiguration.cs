using Microsoft.Extensions.Configuration;
using SpecMemoService;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;

namespace TKE.SC.PIPO
{
    internal class ServiceConfiguration : IServiceConfiguration
    {
        private readonly IConfiguration _configuration;
        public ServiceConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public BasicHttpBinding GetBindings()
        {
            var bindings = new System.ServiceModel.BasicHttpBinding();
            bindings.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            bindings.Security.Mode = (BasicHttpSecurityMode)BasicHttpsSecurityMode.Transport;
            bindings.MaxBufferSize = int.MaxValue;
            bindings.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
            bindings.MaxReceivedMessageSize = int.MaxValue;
            bindings.AllowCookies = true;
            bindings.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

            return bindings;
        }

        public EndpointAddress GetEndpointAddress(string pipoSettings)
        {
            var remoteAddress = GetAppSetting(pipoSettings, Constants.CreateUpdateSpecMemo);

            return new EndpointAddress(remoteAddress);
        }

        public X509ServiceCertificateAuthentication ConfigureNoSslValidation()
        {
            return new X509ServiceCertificateAuthentication()
            {
                CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None,
                RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck
            };
        }

        public SpecMemoOBOM_CreateUpdate_OutClient ConfigureCredentials(SpecMemoOBOM_CreateUpdate_OutClient client, string pipoSettings)
        {
            client.ClientCredentials.UserName.UserName = GetAppSetting(pipoSettings, Constants.Username);
            client.ClientCredentials.UserName.Password = GetAppSetting(pipoSettings, Constants.Password);

            client.ClientCredentials.ServiceCertificate.SslCertificateAuthentication = ConfigureNoSslValidation();

            return client;
        }



        public string GetAppSetting(string pipoSettings, string key)
        {
            var paramSettings = "ParamSettings";
            var value = _configuration[$"{paramSettings}:{pipoSettings}:{key}"];
            if (string.IsNullOrEmpty(value))
            {
                throw new KeyNotFoundException($"Key found in AppSettings: {pipoSettings}:{key}");
            }

            return value;
        }


    }
}
