using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TKE.SC.Common.Model.HttpClientModel;
using TKE.SC.Common.Model.UIModel;
using TKE.CPQ.IdentityServer.Data;
using TKE.SC.Common;

namespace TKE.CPQ.IdentityServer.Extensions


{
    public class GraphQuery : IGraphQuery
    {
        private IConfiguration _configure;
        public GraphQuery(IConfiguration configuration)
        {
            _configure = configuration;
        }

        public async Task<User> GetUserInfoAsync(string emailId)
        {
            var userProfileQueries = new Dictionary<string, string>()
            {
                { "FirstName:givenName",$"users/{emailId}" },
                 { "LastName:surname",$"users/{emailId}" },
                 { "Email:userPrincipalName",$"users/{emailId}" },
                  {  "Groups:value.[id]", $"users/{emailId}/transitiveMemberOf/microsoft.graph.group?$select=id"},
            };
           
            var distintQueries = userProfileQueries.Values.Distinct();

            var accessToken = GetAccessToken();
            var requestModels = distintQueries.Select(_ => GetRequestModel(accessToken, _));

            var graphResults = await ExecuteQueryAsync(requestModels).ConfigureAwait(false);

            var userInfo = JsonConvert.SerializeObject(FormatResult(graphResults, userProfileQueries));
            return JsonConvert.DeserializeObject<User>(userInfo);
        }

        private ExpandoObject FormatResult(IEnumerable<KeyValuePair<string, string>> graphResult,  IDictionary<string, string> inputQueries)
        {
            
            var expando = new ExpandoObject();
            foreach (var key in inputQueries.Keys)
            {
                var currentValue = inputQueries[key];
                var expandoPropertyName = key.Split(":")[0];
                var expandoPropertyValue = ExtractValue(graphResult.FirstOrDefault(x => x.Key.EndsWith(currentValue)).Value, key.Split(":")[1]);
                expando.TryAdd(expandoPropertyName, expandoPropertyValue);

            }

            return expando;
        }

        private object ExtractValue(string result, string propertyName)
        {
            if (string.IsNullOrEmpty(result) || string.IsNullOrEmpty(propertyName))
            {
                return string.Empty;
            }
            Regex regex = new Regex("\\[(?<propertyName>\\w+)\\]");
          
            Match match = regex.Match(propertyName);
            if (match.Success)
            {
                return (ExtractArrayProperty(result,match.Groups["propertyName"].Value));
            }

            return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(result)[propertyName].ToString();
        }

        private IEnumerable<string> ExtractArrayProperty(string result, string arrayProperty)
        {
            var collection = new List<string>();
            var value = JsonConvert.DeserializeObject<List<object>>(JsonConvert.DeserializeObject<Dictionary<string, object>>(result)["value"].ToString());
            foreach (var item in value)
            {
                collection.Add(JsonConvert.DeserializeObject<Dictionary<string, object>>(item.ToString())[arrayProperty].ToString());
            }
            return collection;
        }

        public async Task<IEnumerable<KeyValuePair<string,string>>> ExecuteQueryAsync(IEnumerable<HttpClientRequestModel> requests)
        {
            var queryPromises = requests.Select(_ => Utilities.MakeHttpRequest(_)).ToList();

            var queryResults = await Task.WhenAll(queryPromises).ConfigureAwait(false);

           return  queryResults.Where(_ => _.IsSuccessStatusCode)
                .Select(_ =>
                new KeyValuePair<string, string>(_.RequestMessage.RequestUri.AbsoluteUri, _.Content.ReadAsStringAsync().Result ));
           
        }

        private HttpClientRequestModel GetRequestModel(string accessToken, HTTPMETHODTYPE method = HTTPMETHODTYPE.GET)
        {
            var baseUrl = "https://graph.microsoft.com/v1.0/";
            var request = new HttpClientRequestModel()
            {
                BaseUrl = baseUrl,
                Method = method,
                RequestHeaders = new Dictionary<string, string>()
                {
                    {"Authorization",  $"Bearer {accessToken}"}
                }
            };
            return request;
        }

        private HttpClientRequestModel GetRequestModel(string accessToken, string query, HTTPMETHODTYPE method = HTTPMETHODTYPE.GET)
        {
            var request = GetRequestModel(accessToken, method);
            request.BaseUrl = $"{request.BaseUrl}{query}";
            return request;
        }

        private string GetAccessToken()
        {
           
            var clientId = _configure["GraphAPI:ClientId"];
            var clientSecret = _configure["GraphAPI:ClientSecret"];
            var authority = _configure["GraphAPI:Authority"];

            var app = ConfidentialClientApplicationBuilder.Create(clientId)
                    .WithClientSecret(clientSecret)
                    .WithAuthority(new Uri(authority))
                    .Build();

            string[] scopes = new string[] { $"{_configure["GraphAPI:Scope"]}" };

            AuthenticationResult result = app.AcquireTokenForClient(scopes).ExecuteAsync().Result;
            return result.AccessToken;
        }

       
    }
}
