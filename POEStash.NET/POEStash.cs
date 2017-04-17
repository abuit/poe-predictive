using System;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace POEStash
{
    public class POEStash : IDisposable
    {
        public static POEStash CreateAPIStash()
        {
            return new POEStash(new APIJsonProvider(), new JsonDBCache());
        }

        private readonly IJsonProvider provider;
        private readonly IJsonCache cache;

        private POEStash(IJsonProvider provider, IJsonCache cache)
        {
            this.provider = provider;
            this.cache = cache;
        }

        public void Dispose()
        {
            cache?.Dispose();
            provider?.Dispose();
        }

        public async Task<StashCollection> GetStash(string token)
        {
            string json = await GetJson(token);

            StashCollection collection = JsonConvert.DeserializeObject<StashCollection>(json);

            foreach (Stash stash in collection.Stashes)
            {
                foreach (Item item in stash.Items)
                {
                    item.Stash = stash;
                }
            }

            return collection;
        }

        private async Task<string> GetJson(string token)
        {
            string json = string.Empty;

            if (cache != null)
            {
                json = await cache.GetJson(token);

                if (!string.IsNullOrEmpty(json))
                {
                    return json;
                }
            }

            json = await provider.GetJson(token);
            return json;
        }
    }
}
