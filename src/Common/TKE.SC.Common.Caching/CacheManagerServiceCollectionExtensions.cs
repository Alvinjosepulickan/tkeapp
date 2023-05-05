using Microsoft.Extensions.DependencyInjection;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;

namespace TKE.SC.Common.Caching
{
    public static class CacheManagerServiceCollectionExtensions
    {
        public static void AddCacheManager(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ICacheManager, CpqCacheManager>();
        }
    }
}
