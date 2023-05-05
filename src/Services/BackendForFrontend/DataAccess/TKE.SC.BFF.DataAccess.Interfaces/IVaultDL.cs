using System.IO;
using System.Threading.Tasks;
using TKE.SC.Common.Model.VaultModel;

namespace TKE.SC.BFF.DataAccess.Interfaces
{
    public interface IVaultDL
    {
        Task<string> GetToken();
        Task<string> GetFolderPath(string projectId);

        Task<string> UploadDocument(byte[] fileStream, VaultUploadModel fileName);
    }
}