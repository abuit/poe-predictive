using System;
using System.Threading.Tasks;

namespace POEStash.Model.Providers
{
    public interface IJsonProvider : IDisposable
    {
        Task<JsonPOEBucket> GetBucket(string token);
    }
}
