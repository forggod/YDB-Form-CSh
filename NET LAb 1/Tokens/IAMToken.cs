using Jose;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;

namespace NET_LAb_1.Tokens
{
    internal class IAMToken
    {
        public async Task<string?> createAsync(string privateKey, string keyId, string serviceAccountId)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var headers = new Dictionary<string, object>()
            {
                { "kid", keyId }
            };
            var payload = new Dictionary<string, object>()
            {
                { "aud", "https://iam.api.cloud.yandex.net/iam/v1/tokens" },
                { "iss", serviceAccountId },
                { "iat", now },
                { "exp", now + 3600 }
            };
            using (var rsa = RSA.Create())
            {
                rsa.ImportFromPem(privateKey.ToCharArray());
                string encodedToken = JWT.Encode(payload, rsa, JwsAlgorithm.PS256, headers);
                string? iamToken = await PostRequestToYandexIam(encodedToken);
                return iamToken;
            }
        }

        static async Task<string?> PostRequestToYandexIam(string jwtToken)
        {
            using (var httpClient = new HttpClient())
            {
                var requestData = new { jwt = jwtToken };
                var jsonContent = JsonConvert.SerializeObject(requestData);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var url = "https://iam.api.cloud.yandex.net/iam/v1/tokens";
                var response = await httpClient.PostAsync(url, httpContent);
                string? result = null;
                if (response.IsSuccessStatusCode)
                {
                    var responsJson = await response.Content.ReadAsStringAsync();
                    if (Config.IsWriteResponse)
                        Console.WriteLine("Response: " + responsJson);
                    JObject jsonObject = JObject.Parse(responsJson);
                    result = jsonObject["iamToken"].ToString();
                }
                else
                {
                    Console.WriteLine("Error: " + response.StatusCode);
                }
                return result;
            }
        }
    }
}

