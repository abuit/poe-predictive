using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace POEStash.Model.Providers
{
    public class DatabaseProvider : IDatabaseProvider
    {
        private const string DB_NAME = @"poestash.db";
        private const string STASH_COLLECTION_NAME = @"stashes";

        public Task UpdateDatabase(JsonPOEBucket bucket)
        {
            return Task.Run(() =>
            {
                using (LiteDatabase db = new LiteDatabase(Path.GetTempPath() + DB_NAME))
                {
                    var stashCollection = db.GetCollection<JsonPOEStash>(STASH_COLLECTION_NAME);

                    foreach (var stash in bucket.Stashes)
                    {
                        stashCollection.Upsert(stash);
                    }
                }
            });
        }

        public Task<Snapshot> GetSnapshot()
        {
            return Task.Run(() =>
            {
                using (LiteDatabase db = new LiteDatabase(Path.GetTempPath() + DB_NAME))
                {
                    var stashCollection = db.GetCollection<JsonPOEStash>(STASH_COLLECTION_NAME);

                    List<JsonPOEStash> all = stashCollection.FindAll().ToList();

                    return new Snapshot(all);
                }
            });
        }
    }
}