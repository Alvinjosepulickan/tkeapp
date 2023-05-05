using SpecMemoService;
using System.ServiceModel;
using System.ServiceModel.Security;

namespace TKE.SC.PIPO
{
    internal interface IServiceConfiguration
    {
        SpecMemoOBOM_CreateUpdate_OutClient ConfigureCredentials(SpecMemoOBOM_CreateUpdate_OutClient client, string pipoSettings);
        X509ServiceCertificateAuthentication ConfigureNoSslValidation();
        string GetAppSetting(string pipoSettings, string key);
        BasicHttpBinding GetBindings();
        EndpointAddress GetEndpointAddress(string pipoSettings);
    }
}