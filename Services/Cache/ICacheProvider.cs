using System.Threading.Tasks;

namespace kroniiapi.Services
{
    public interface ICacheProvider
    {
        Task<T> GetFromCache<T>(string key) where T : class;
        Task SetCache<T>(string key, T value) where T : class;
        Task ClearCache(string key);
    }
}