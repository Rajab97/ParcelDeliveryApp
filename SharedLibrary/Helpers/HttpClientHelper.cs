using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Helpers
{
    public static class HttpClientHelper
    {
        public static async Task<string> GetQueryStringFromModelAsync<T>(T model, string baseUrl)
        {
            var queryParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(
            JsonConvert.SerializeObject(model));

            // Add the query parameters to the URL
            var queryStringContent = new FormUrlEncodedContent(queryParams);
            var queryString = await queryStringContent.ReadAsStringAsync();
            var requestUrl = baseUrl + "?" + queryString;
            return requestUrl;
        }
    }
}
