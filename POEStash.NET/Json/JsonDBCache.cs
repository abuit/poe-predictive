using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LiteDB;
using Newtonsoft.Json;

namespace POEStash
{
    public class JsonDBCache : IJsonCache
    {
        private const string BUCKET_ID_COLLECTION_NAME = "bucketID";
        private const string BUCKETS_COLLECTION_NAME = "buckets";

        private const string API_URL = "http://www.pathofexile.com/api/public-stash-tabs?id=";

        private readonly HttpClient httpClient;
        private readonly LiteDatabase db;

        private string DbLocation => Path.GetTempPath() + "POEStash.db";

        public JsonDBCache()
        {
            httpClient = new HttpClient();
            db = new LiteDatabase(DbLocation);
            Start();
        }

        public void Start()
        {
            Task.Run(async () => await DoCycle());
        }

        public async Task<string> GetJson(string id)
        {
            var collection = db.GetCollection<IndexedBucket>(BUCKETS_COLLECTION_NAME);
            if (collection.Exists(x => x.ChangeID == id))
            {
                var bucket = collection.Find(x => x.ChangeID == id).FirstOrDefault();
                return await Task.Run(() => bucket.Json);
            }

            return await CycleImmediate(id);
        }

        private async Task DoCycle()
        {
            BucketID bucketID = GetLastBucket();

            string id = bucketID.NextChangeID;
            string previousID = bucketID.ChangeID;

            while (true)
            {
                string newID = await Cycle(id, previousID);
                previousID = id;
                id = newID;
            }
        }

        private BucketID GetLastBucket()
        {
            var collection = db.GetCollection<BucketID>(BUCKET_ID_COLLECTION_NAME);

            BucketID id = collection.FindById("0");
            if (id == null)
            {
                id = new BucketID("0", "0", "0");
                Debug.WriteLine("Found no last bucket, providing default.");
            }
            Debug.WriteLine($"Got last BucketID {id}");
            return id;
        }

        private void SetLastBucket(BucketID id)
        {
            var collection = db.GetCollection<BucketID>(BUCKET_ID_COLLECTION_NAME);
            collection.Upsert("0", id);
            //Debug.WriteLine($"Last BucketID is now {id}");
        }

        private async Task<string> Cycle(string id, string previousID)
        {
            Stopwatch sw = Stopwatch.StartNew();

            string json = await httpClient.GetStringAsync(API_URL + id);

            TimeSpan fetchTime = sw.Elapsed;

            StashCollection collection = JsonConvert.DeserializeObject<StashCollection>(json);

            var bucket = new IndexedBucket(id, collection.NextChangeID, previousID, json);

            var col = db.GetCollection<IndexedBucket>(BUCKETS_COLLECTION_NAME);

            // Early out if this ID is already indexed
            if (col.Exists(x => x.ChangeID == id))
            {
                // We may have hit an ID that was ImmediateCycled, in that case we need to make sure it has the right previous ID
                if (string.IsNullOrEmpty(bucket.PreviousChangeID))
                {
                    bucket.PreviousChangeID = previousID;
                }

                col.Update(bucket);

                sw.Stop();
                Debug.WriteLine($"{id} already exists. Cycle time: {sw.Elapsed}");
                return collection.NextChangeID;
            }

            col.Insert(bucket);

            sw.Stop();

            SetLastBucket(new BucketID(id, collection.NextChangeID, previousID));

            Debug.WriteLine($"Indexed {id}. Fetch Time: {fetchTime} Cycle time: {sw.Elapsed}");

            return collection.NextChangeID;
        }

        private async Task<string> CycleImmediate(string id)
        {
            Stopwatch sw = Stopwatch.StartNew();

            string json = await httpClient.GetStringAsync(API_URL + id);

            TimeSpan fetchTime = sw.Elapsed;

            StashCollection collection = JsonConvert.DeserializeObject<StashCollection>(json);

            var bucket = new IndexedBucket(id, collection.NextChangeID, "", json);

            var col = db.GetCollection<IndexedBucket>(BUCKETS_COLLECTION_NAME);

            // Early out if this ID is already indexed
            if (col.Exists(x => x.ChangeID == id))
            {
                sw.Stop();
                Debug.WriteLine($"{id} already exists. Cycle time: {sw.Elapsed}");
                return collection.NextChangeID;
            }

            col.Insert(bucket);

            sw.Stop();

            Debug.WriteLine($"ImmediateCycled {id}. Fetch Time: {fetchTime} Cycle time: {sw.Elapsed}");

            return json;
        }

        public void Dispose()
        {
            db?.Dispose();
            httpClient?.Dispose();
        }
    }

    public class BucketID
    {
        [BsonId]
        public string ChangeID { get; set; }

        public string PreviousChangeID { get; set; }

        public string NextChangeID { get; set; }

        public BucketID()
        {
        }

        public BucketID(string changeId, string nextChangeId, string previousChangeId)
        {
            ChangeID = changeId;
            PreviousChangeID = previousChangeId;
            NextChangeID = nextChangeId;
        }

        public override string ToString()
        {
            return $"{ChangeID} (Previous: {PreviousChangeID}, Next: {NextChangeID})";
        }
    }

    public class IndexedBucket
    {
        [BsonId]
        public string ChangeID { get; set; }

        public string PreviousChangeID { get; set; }
        public string NextChangeID { get; set; }
        public string Json { get; set; }

        public IndexedBucket()
        {
        }

        public IndexedBucket(string changeId, string nextChangeId, string previousChangeId, string json)
        {
            ChangeID = changeId;
            NextChangeID = nextChangeId;
            PreviousChangeID = previousChangeId;
            Json = json;
        }
    }
}