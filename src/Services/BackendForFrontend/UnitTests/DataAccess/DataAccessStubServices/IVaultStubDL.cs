using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model.VaultModel;

namespace TKE.SC.BFF.UnitTests.DataAccess.DataAccessStubServices
{
    class IVaultStubDL : IVaultDL
    {
        public async Task<string> GetFolderPath(string projectId)
        {
            return "";
        }

        public async Task<string> GetToken()
        {
            return "";
        }

        public async Task<string> UploadDocument(byte[] fileStream, VaultUploadModel fileName)
        {
            return "";
        }
    }
}
