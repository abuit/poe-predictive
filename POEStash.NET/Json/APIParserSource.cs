using System.Threading.Tasks;
using System.Net.Http;

namespace POEStash
{
    public class APIJsonProvider : IJsonProvider
    {
        private const string API_URL = "http://www.pathofexile.com/api/public-stash-tabs?id=";
        private readonly string id;
        private readonly JsonCache cache;

        public APIJsonProvider(string id)
        {
            this.id = id;
            this.cache = new JsonCache();
        }

        public async Task<string> GetJson()
        {
            string json = string.Empty;

            if (cache.HasJsonForKey(id))
            {
                json = await cache.LoadJsonFromCache(id);
                return json;
            }
            
            using (var httpClient = new HttpClient())
            {
                json = await httpClient.GetStringAsync(API_URL + id);
                await cache.Cache(id, json);
            }

            return json;
        }
    }
}
