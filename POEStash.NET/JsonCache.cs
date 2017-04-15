using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace POEStash
{
    public class JsonCache
    {
        private const string PREFIX = "POESTASH";
        private Dictionary<string, CachedJson> cache;

        public JsonCache()
        {
            cache = new Dictionary<string, CachedJson>();
            IndexCache();
        }

        private void IndexCache()
        {
            var tempFolder = Path.GetTempPath();
            DirectoryInfo dirInfo = new DirectoryInfo(tempFolder);
            foreach (FileInfo file in dirInfo.EnumerateFiles())
            {
                if (file.Name.StartsWith(PREFIX))
                {
                    string id = file.Name.Substring(PREFIX.Length + 1, file.Name.Length - PREFIX.Length - 1);
                    cache.Add(id, new CachedJson(id));

                    System.Console.WriteLine("Indexed file: " + file.Name);
                }
            }
        }

        public async Task Cache(string id, string json)
        {
            await SaveToFile(id, json);
            cache.Add(id, new CachedJson(id, json));
        }

        public async Task<string> LoadJsonFromCache(string id)
        {
            string json = string.Empty;
            if (!HasJsonForKey(id))
            {
                return json;
            }

            if (cache[id].Loaded)
            {
                json = cache[id].Json;
            }
            else
            {
                json = await LoadFromFile(id);

                if (!string.IsNullOrEmpty(json))
                {
                    cache[id].Json = json;
                    cache[id].Loaded = true;
                }
            }

            return json;
        }

        private async Task<string> LoadFromFile(string id)
        {
            string json = string.Empty;
            var filePath = Path.GetTempPath() + PREFIX + "-" + id;

            if (!File.Exists(filePath))
            {
                return json;
            }

            System.Console.WriteLine("Loading file from cache: " + filePath);

            using (StreamReader reader = new StreamReader(filePath))
            {
                json = await reader.ReadToEndAsync();
            }
            return json;
        }

        private async Task SaveToFile(string id, string json)
        {
            var filePath = Path.GetTempPath() + PREFIX + "-" + id;

            System.Console.WriteLine("Writing file to cache: " + filePath);

            using (Stream stream = File.OpenWrite(filePath))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    await writer.WriteAsync(json);
                }
            }
        }

        public bool HasJsonForKey(string id)
        {
            return cache.ContainsKey(id);
        }

        private class CachedJson
        {
            public string ID { get; }
            public bool Loaded { get; set; }
            public string Json { get; set; }

            public CachedJson(string id)
            {
                ID = id;
                Json = string.Empty;
                Loaded = false;
            }

            public CachedJson(string id, string json)
            {
                ID = id;
                Json = json;
                Loaded = true;
            }
        }
    }
}