using System.Collections;
using System.Collections.Generic;

namespace POEStash.Model
{
    public class Snapshot : IEnumerable<JsonPOEStash>
    {
        private readonly IEnumerable<JsonPOEStash> stashes;

        public Snapshot(IEnumerable<JsonPOEStash> stashes)
        {
            this.stashes = stashes;
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