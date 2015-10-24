using Jagi.Database.Models;
using Jagi.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Jagi.Database.Cache
{
    public class InitCodeCache : IRunAtStartup
    {
        private DataContext _context;
        private CodeCache _cache;

        public InitCodeCache(DataContext context)
        {
            _context = context;
            _cache = new CodeCache(MemoryCache.Default);
        }

        public void Execute()
        {
            var codes = _context.CodeFiles.AsNoTracking().ToList();
            foreach (var code in codes)
            {
                var details = code.CodeDetails.ToList();
                _cache.Set(code, details);
            }
        }
    }
}
