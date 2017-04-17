using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace POEStash.Model
{
    public class Snapshot : IEnumerable<JsonPOEStash>
    {
        private static int snapshotId;

        private readonly IEnumerable<JsonPOEStash> stashes;

        public int Id { get; }

        public int AmountOfItems { get; }

        public int[] AmountsPerType { get; }

        public Snapshot(IEnumerable<JsonPOEStash> stashes)
        {
            this.stashes = stashes;
            this.Id = Snapshot.snapshotId++;

            AmountOfItems = 0;
            AmountsPerType = new int[(int) ItemType.COUNT];
            foreach (var stash in stashes)
            {
                foreach (var item in stash.Items)
                {
                    AmountsPerType[(int)item.ItemType]++;
                    AmountOfItems++;
                }
            }
        }

        public IEnumerator<JsonPOEStash> GetEnumerator()
        {
            foreach (JsonPOEStash stash in stashes)
            {
                yield return stash;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return stashes.GetEnumerator();
        }
    }
}