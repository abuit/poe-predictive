using Newtonsoft.Json;
using System.Threading.Tasks;

namespace POEStash
{
    public class POEStash
    {
        public static POEStash CreateAPIStash(string id = "0")
        {
            return new POEStash(new APIJsonProvider(id));
        }

        public static POEStash CreateJsonStash(string json)
        {
            return new POEStash(new JsonStringProvider(json));
        }

        public static POEStash CreateFileStash(string filePath)
        {
            return new POEStash(new FileJsonProvider(filePath));
        }

        private readonly IJsonProvider parserSource;

        private POEStash(IJsonProvider parserSource)
        {
            this.parserSource = parserSource;
        }

        public async Task<StashCollection> GetStashes()
        {
            string json = await parserSource.GetJson();

            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            var settings = new JsonSerializerSettings()
            {
            };

            StashCollection collection = JsonConvert.DeserializeObject<StashCollection>(json, settings);

            foreach (Stash stash in collection.Stashes)
            {
                foreach (Item item in stash.Items)
                {
                    item.Stash = stash;
                }
            }

            return collection;
        }
    }
}
