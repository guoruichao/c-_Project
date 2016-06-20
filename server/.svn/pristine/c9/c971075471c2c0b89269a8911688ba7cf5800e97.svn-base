
namespace libDB
{
    public interface ICache
    {
        object Get(string cacheKey);
        void Set(string cacheKey, object cacheData, double cacheMinutes);
        void Set(string cacheKey, string dependFile, object cacheData);
        void Set(string cacheKey, object cacheData);
        void Remove(string cacheKey);
        void RemoveAll();
    }
}
