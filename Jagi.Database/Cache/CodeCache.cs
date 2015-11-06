using Jagi.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Jagi.Database.Cache
{
    public class CodeCache : CacheBase
    {
        // 非常重要的命名原則，不可以更改！
        private const string REGION_NAME = "CODE";
        private const string KEY_DETAIL_CODE = "!Detail:";
        public CodeCache() : base(REGION_NAME) {
            _cache = MemoryCache.Default;
        }

        public CodeCache(MemoryCache mainCache)
            : base(REGION_NAME)
        {
            _cache = mainCache;
        }
        /// <summary>
        /// Key Definition: ItemType.ItemCode.ParentCode
        /// if not PrarentCode, will be ItemType.ItemCode.NULL
        /// 例外設定：如果沒有 Details 則不會增加
        /// </summary>
        /// <param name="code"></param>
        /// <param name="codeDetails"></param>
        public void Set(CodeFile code, IEnumerable<CodeDetail> codeDetails)
        {
            if (codeDetails == null)
                return;

            foreach (var detail in codeDetails)
            {
                CodeMapper item = new CodeMapper
                {
                    ItemType = code.ITEM_TYPE,
                    ParentCode = code.PARENT_CODE,
                    ItemCode = detail.ITEM_CODE,
                    Description = detail.DESC,
                    OrderByNumber = code.CHAR_NUM,
                };
                Set(GetKeyCode(code.ITEM_TYPE, detail.ITEM_CODE, code.PARENT_CODE), item);
            }
        }

        /// <summary>
        /// 移除 CodeFile 與其 Details 相關的資料 (多筆)
        /// 但如果沒有 Details 則不移除
        /// </summary>
        /// <param name="code">CodeFile</param>
        /// <param name="codeDetails">CodeFile relative Details</param>
        public void Remove(CodeFile code, IEnumerable<CodeDetail> codeDetails)
        {
            if (codeDetails == null)
                return;

            foreach (var detail in codeDetails)
            {
                Remove(GetKeyCode(code.ITEM_TYPE, detail.ITEM_CODE, code.PARENT_CODE));
            }
        }

        /// <summary>
        /// 移除 CodeDetails (單筆)
        /// </summary>
        /// <param name="code">CodeFile</param>
        /// <param name="codeDetails">CodeFile relative Details</param>
        public void Remove(string itemType, string itemCode, string parentCode)
        {
            Remove(GetKeyCode(itemType, itemCode, parentCode));
        }

        /// <summary>
        /// 取得 Detail 的代碼名稱
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="itemCode">必須填入，如果沒有值，回傳空白字串</param>
        /// <param name="parentCode">可以是空白，但請盡量給值，否則速度很慢</param>
        /// <returns></returns>
        public string GetCodeDesc(string itemType, string itemCode,
            string parentCode = null)
        {
            if (string.IsNullOrEmpty(itemCode))
                return string.Empty;

            var key = GetKeyCode(itemType, itemCode, parentCode);
            var result = Get<CodeMapper>(key);
            if (result == null)
            {
                var details = GetCodeDetails(itemType, parentCode);
                result = details.FirstOrDefault(k => k.ItemCode == itemCode);
            }
            return result != null ? result.Description : string.Empty;
        }

        /// <summary>
        /// 取得 CodeFile 所有的 Detail，用 Dictionary 呈現
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetDetails(string itemType, string parentCode = null)
        {
            var codeDetails = GetCodeDetails(itemType, parentCode);
            return codeDetails.OrderBy(o => o.ItemCode).Select(s => new { Key = s.ItemCode, Value = s.Description })
                .ToDictionary(k => k.Key, v => v.Value);
        }

        /// <summary>
        /// 取得 CodeFile 所有的 Detail 用 CodeMapper 呈現
        /// 注意大小寫必須要一致
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        public IEnumerable<CodeMapper> GetCodeDetails(string itemType, string parentCode = null)
        {
            itemType = itemType.ToUpper();
            var key = string.Empty;
            if (string.IsNullOrEmpty(parentCode))
            {
                key = "=" + itemType + ".";
            }
            else
                key = itemType + "." + parentCode + ".";
            var result = GetAll().Where(k => k.Key.Contains(key)).Select(s => (CodeMapper)s.Value);

            return result;
        }

        private string GetKeyCode(string itemType, string itemCode, string parentCode)
        {
            return itemType.ToUpper() + "."
                + (string.IsNullOrEmpty(parentCode) ? "NULL" : parentCode) + "."
                + KEY_DETAIL_CODE + itemCode;
        }
    }

    public class CodeMapper
    {
        public string ItemType { get; set; }
        public string ItemCode { get; set; }
        public string ParentCode { get; set; }
        public int OrderByNumber { get; set; }
        public string Description { get; set; }
    }
}
