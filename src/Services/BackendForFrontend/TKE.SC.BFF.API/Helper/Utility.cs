using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using TKE.SC.Common;

namespace TKE.SC.BFF.Helper
{
    public class Utility : Utilities
    {
        public static string GetSessionId(ClaimsPrincipal userClaims) => GetClaim(userClaims, "SessionId");

        public static string GetClaim(ClaimsPrincipal principal, string claimName) => GetClaims(principal, claimName).FirstOrDefault();

        public static IList<string> GetClaims(ClaimsPrincipal principal, string claimName) => (from claim in principal.Claims
                                                                                               where claim.Type.Equals(claimName, System.StringComparison.InvariantCultureIgnoreCase)
                                                                                               select claim.Value).ToList();

        /// <summary>
        /// Serializes object to a string
        /// </summary>
        /// <param Name="value"></param>
        /// <returns></returns>
        public static string SerializeObjectValue(object value)
        {
            return JsonConvert.SerializeObject(value, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        /// <summary>
        /// Deserialize string Value to the specified object
        /// </summary>
        /// <typeparam Name="T"></typeparam>
        /// <param Name="value"></param>
        /// <returns></returns>
        public static T DeserializeObjectValue<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// String comparision
        /// </summary>
        /// <param Name="valueOne"></param>
        /// <param Name="valueTwo"></param>
        /// <returns></returns>
        public static bool CheckEquals(string valueOne, string valueTwo)
        {
            return !string.IsNullOrEmpty(valueOne) && !string.IsNullOrEmpty(valueTwo) &&
                   valueOne.Trim().ToUpperInvariant() == valueTwo.Trim().ToUpperInvariant();
        }
    }
}
