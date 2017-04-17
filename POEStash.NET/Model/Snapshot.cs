using System.Collections;
using System.Collections.Generic;

namespace POEStash.Model
{
    public class Snapshot : IEnumerable<JsonPOEStash>
    {
        private static int snapshotId;

        private readonly IEnumerable<JsonPOEStash> stashes;

        public int Id { get; }

        public Snapshot(IEnumerable<JsonPOEStash> stashes)
        {
            this.stashes = stashes;
            this.Id = Snapshot.snapshotId++;
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