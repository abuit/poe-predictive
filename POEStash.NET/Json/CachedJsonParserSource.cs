using System.Threading.Tasks;

namespace POEStash
{
    public class JsonStringProvider : IJsonProvider
    {
        private readonly string json;

        public JsonStringProvider(string json)
        {
            this.json = json;
        }

        public async Task<string> GetJson()
        {
            return await Task.Run(() => json);
        }
    }
}
