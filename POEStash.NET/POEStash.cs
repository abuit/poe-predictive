using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POEStash.Model;
using POEStash.Model.Providers;

namespace POEStash
{
    public static class POEStash
    {
        private const int RATE_LIMIT = 60000;
        private const string FALLBACK_ID = "45339225-47984329-44953841-51680241-48352319";
        public static IJsonProvider JsonProvider { get; set; } = new APIJsonProvider();
        public static IDatabaseProvider DatabaseProvider = new DatabaseProvider();

        public static void Start()
        {
            Task.Run(async () =>
            {
                string id = GetLastID();

                while (true)
                {
                    id = await Cycle(id);
                    SetLastID(id);
                    await Task.Delay(RATE_LIMIT);
                }
            });
        }

        public static async Task<Snapshot> GetSnapshot()
        {
            return await DatabaseProvider.GetSnapshot();
        }

        public static async Task<string> Cycle(string id)
        {
            Console.WriteLine("Cycle ID: " + id);

            Stopwatch sw = Stopwatch.StartNew();
            var bucket = await TimeAction(async () => await JsonProvider.GetBucket(id), "Json.GetBucket");
            //var bucket = await JsonProvider.GetBucket(id);

            Console.WriteLine(
                $"Cycle contains {bucket.Stashes.Length} stashes with a total of {bucket.Stashes.Sum(x => x.Items.Length)} items.");
            //Debug.WriteLine($"Input has {bucket.Stashes.Length} stashes. Total of {bucket.Stashes.Sum(x => x.Items.Length)} items.");

            await TimeAction(async () => await DatabaseProvider.UpdateDatabase(bucket), "UpdateDatabase");
            //await DatabaseProvider.UpdateDatabase(bucket);

            //var snapshot = await DatabaseProvider.GetSnapshot();
            Snapshot snapshot = await TimeAction(async () => await DatabaseProvider.GetSnapshot(), "GetSnapshot");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Snapshot contains {snapshot.AmountOfItems} items.");
            for (var index = 0; index < snapshot.AmountsPerType.Length; index++)
            {
                var count = snapshot.AmountsPerType[index];
                sb.AppendLine($"    {(ItemType) index} Count: {count}");
            }
            Console.WriteLine(sb.ToString());

            sw.Stop();
            Debug.WriteLine($"Total Cycle time: {sw.Elapsed}");
            RaiseOnStashUpdated(new SnapShotEventArgs(snapshot));

            return bucket.NextChangeID;
        }

        private static async Task TimeAction(Func<Task> action, string label)
        {
            Stopwatch sw = Stopwatch.StartNew();
            await action();
            sw.Stop();
            Debug.WriteLine($"{label} Cycle time: {sw.Elapsed}");
        }

        private static async Task<T> TimeAction<T>(Func<Task<T>> action, string label)
        {
            Stopwatch sw = Stopwatch.StartNew();
            T result = await action();
            sw.Stop();
            Debug.WriteLine($"{label} Cycle time: {sw.Elapsed}");
            return result;
        }

        private static string GetLastID()
        {
            try
            {
                var result = File.ReadAllLines($"{Path.GetTempPath()}poestashid.txt");
                if (result.Length > 0)
                {
                    return result[0];
                }
                else
                {
                    return FALLBACK_ID;
                }
            }
            catch (Exception exception)
            {
                if (!(exception is FileNotFoundException)) // This is OK, the file will be created at a later point
                {
                    Debug.WriteLine(exception);
                }
            }

            return FALLBACK_ID;
        }

        private static void SetLastID(string id)
        {
            try
            {
                File.WriteAllLines($"{Path.GetTempPath()}poestashid.txt", new[] {id});
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public static event EventHandler<SnapShotEventArgs> OnStashUpdated;

        private static void RaiseOnStashUpdated(SnapShotEventArgs e)
        {
            OnStashUpdated?.Invoke(null, e);
        }
    }

    public class SnapShotEventArgs : EventArgs
    {
        public Snapshot Snapshot { get; }

        public SnapShotEventArgs(Snapshot snapshot)
        {
            Snapshot = snapshot;
        }
    }
}