using System.Threading.Tasks;

namespace POEStash.Model.Providers
{
    public interface IDatabaseProvider
    {
        Task UpdateDatabase(JsonPOEBucket bucket);
        Task<Snapshot> GetSnapshot();
    }
}