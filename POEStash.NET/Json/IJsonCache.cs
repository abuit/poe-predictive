using System;
using System.Threading.Tasks;

namespace POEStash
{
    public interface IJsonCache : IDisposable
    {
        Task<string> GetJson(string token);
    }
}