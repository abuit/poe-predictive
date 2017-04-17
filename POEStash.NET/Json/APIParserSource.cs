using System.Threading.Tasks;
using System.Net.Http;

namespace POEStash
{
    public class APIJsonProvider : IJsonProvider
    {
        private const string API_URL = "http://www.pathofexile.com/api/public-stash-tabs?id=";

        public void Dispose()
        {
        }

        public async Task<string> GetJson(string token)
        {
            string json = string.Empty;

            if (JsonFileCache.HasJsonForKey(token))
            {
                json = await JsonFileCache.LoadJsonFromCache(token);
                return json;
            }
            
            using (var httpClient = new HttpClient())
            {
                json = await httpClient.GetStringAsync(API_URL + token);
                await JsonFileCache.Cache(token, json);
            }

            return json;
        }
    }
}
