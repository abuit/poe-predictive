using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using POEStash.Model.Providers;

namespace POEStash.Model
{
    public class APIJsonProvider : IJsonProvider
    {
        private const string API_URL = "http://www.pathofexile.com/api/public-stash-tabs?id=";

        public void Dispose()
        {
        }

        public async Task<JsonPOEBucket> GetBucket(string token)
        {
            using (var httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync(API_URL + token);
                JsonPOEBucket entry = JsonConvert.DeserializeObject<JsonPOEBucket>(json);
                return entry;
            }
        }
    }
}
