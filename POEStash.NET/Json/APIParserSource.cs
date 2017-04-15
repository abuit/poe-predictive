using System.Threading.Tasks;
using System.Net.Http;

namespace POEStash
{
    public class APIJsonProvider : IJsonProvider
    {
        private const string API_URL = "http://www.pathofexile.com/api/public-stash-tabs?id=";
        private readonly string id;

        public APIJsonProvider(string id)
        {
            this.id = id;
        }

        public async Task<string> GetJson()
        {
            string json = string.Empty;

            if (JsonCache.HasJsonForKey(id))
            {
                json = await JsonCache.LoadJsonFromCache(id);
                return json;
            }
            
            using (var httpClient = new HttpClient())
            {
                json = await httpClient.GetStringAsync(API_URL + id);
                await JsonCache.Cache(id, json);
            }

            return json;
        }
    }
}
