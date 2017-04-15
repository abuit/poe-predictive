using System.Threading.Tasks;

namespace POEStash
{
    public interface IJsonProvider
    {
        Task<string> GetJson();
    }
}
