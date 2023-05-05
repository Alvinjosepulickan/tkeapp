using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TKE.SC.Common.Model.HttpClientModel;
using TKE.SC.Common.Model.UIModel;
using TKE.CPQ.IdentityServer.Data;

namespace TKE.CPQ.IdentityServer.Extensions
{
    public interface IGraphQuery
    {
        public Task<User> GetUserInfoAsync(string emailId);

        public Task<IEnumerable<KeyValuePair<string, string>>> ExecuteQueryAsync(IEnumerable<HttpClientRequestModel> requestModels);
    }
}
