using System.Threading.Tasks;
using static TKE.SC.PIPO.Constants;

namespace TKE.SC.PIPO
{
    public interface IPipoConnector
    {
        Task<bool> CreateOrUpdateSpecMemoAsync(string specMemoRequestXml,  ContentType contentType, string pipoSettings);
    }
}