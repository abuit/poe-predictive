using System.IO;
using System.Threading.Tasks;

namespace POEStash
{
    public class FileJsonProvider : IJsonProvider
    {
        private readonly string filePath;

        public FileJsonProvider(string filePath)
        {
            this.filePath = filePath;
        }

        public Task<string> GetJson()
        {
            string json;
            using (StreamReader r = new StreamReader(filePath))
            {
                json = r.ReadToEnd();
            }
            return Task.Run(() => json);
        }
    }
}
