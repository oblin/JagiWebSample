using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace Jagi.Database.Cache
{
    public abstract class CacheBase : IEnumerable<KeyValuePair<string, object>>
    {
        protected String Region { get; set; }
        protected MemoryCache _cache;
        //private Log _log;

        public CacheBase(string region)
        {
            _cache = MemoryCache.Default;
            Region = region;
            //_log = new Log();
        }

        // 因為衍生的 Cache 會自行時做自己的 Get Code 因此不在提供 
        //public CacheItem GetCacheItem(string key)
        //{
        //    string regionKey = CreateKeyWithRegion(key, Region);
        //    CacheItem temporary = _cache.GetCacheItem(regionKey);
        //    return new CacheItem(key, temporary.Value, Region);
        //}

        public long Count()
        {
            return GetAll().Count();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return GetAll().GetEnumerator();
        }

        protected void Set(CacheItem item, CacheItemPolicy policy)
        {
            Set(item.Key, item.Value, policy);
        }

        protected void Set(string key, object value, DateTimeOffset absoluteExpiration)
        {
            Set(key, value, new CacheItemPolicy { AbsoluteExpiration = absoluteExpiration });
        }

        protected void Set(string key, object value, CacheItemPolicy policy)
        {
            _cache.Set(CreateKeyWithRegion(key, Region), value, policy);
        }

        protected void Set(int key, object value)
        {
            Set(key.ToString(), value);
        }

        protected void Set(string key, object value)
        {
            Set(key, value, new CacheItemPolicy());
        }

        protected object Get(string key)
        {
            return _cache.Get(CreateKeyWithRegion(key, Region));
        }

        protected T Get<T>(string key)
        {
            return (T)Get(key);
        }

        protected object Remove(string key)
        {
            return _cache.Remove(CreateKeyWithRegion(key, Region));
        }

        protected DefaultCacheCapabilities DefaultCacheCapabilities
        {
            get
            {
                return (_cache.DefaultCacheCapabilities | System.Runtime.Caching.DefaultCacheCapabilities.CacheRegions);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private string CreateKeyWithRegion(string key, string region)
        {
            return "region:" + Region + ";key=" + key;
        }

        protected IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            return _cache.Where(k => k.Key.Contains("region:" + Region + ";key="))
                .Select(s => new KeyValuePair<string, object>(s.Key, s.Value));
        }
    }
}