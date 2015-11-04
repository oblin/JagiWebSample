using Jagi.Interface;
using System.Linq;
using System.Runtime.Caching;

namespace Jagi.Database.Cache
{
    public class CodeCacheManager : IRunAtStartup
    {
        private DataContext _context;
        private CodeCache _cache;

        public CodeCacheManager(DataContext context)
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

        public void RemoveCodeFile(int id)
        {
            var code = _context.CodeFiles.FirstOrDefault(p => p.ID == id);
            _cache.Remove(code, code.CodeDetails);
        }


        public void RemoveCodeDetail(int id)
        {
            var detail = _context.CodeDetails.FirstOrDefault(p => p.ID == id);
            var code = _context.CodeFiles.FirstOrDefault(p => p.ID == detail.CodeFileID);
            _cache.Remove(code.ITEM_TYPE, detail.ITEM_CODE, code.PARENT_CODE);
        }

        public void SetCodeFile(int id)
        {
            var code = _context.CodeFiles.FirstOrDefault(p => p.ID == id);
            _cache.Set(code, code.CodeDetails);
        }

        public void SetCodeDetail(int id)
        {
            var detail = _context.CodeDetails.FirstOrDefault(p => p.ID == id);
            var code = _context.CodeFiles.FirstOrDefault(p => p.ID == detail.CodeFileID);
            _cache.Set(code, code.CodeDetails);
        }
    }
}
