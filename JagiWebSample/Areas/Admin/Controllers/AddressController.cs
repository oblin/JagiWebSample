using Jagi.Interface;
using Jagi.Helpers;
using Jagi.Mvc.Helpers;
using JagiWebSample.Areas.Admin.Models;
using System.Web.Mvc;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading;
using System;
using System.Data.Entity;

namespace JagiWebSample.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0)]
    public class AddressController : ControllerBase
    {
        private AdminDataContext _context;

        public AddressController(AdminDataContext context)
        {
            _context = context;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult Get(PageInfo pageInfo)
        {
            return GetJsonResult(() =>
            {
                int pageNumber = pageInfo.PageNumber < 1 ? 1 : pageInfo.PageNumber;
                pageInfo.SortField = pageInfo.SortField ?? "Zip";
                int count;
                var fileterResult = GetStartWithFilter(_context.Address, pageInfo.SearchField, pageInfo.SearchKeyword, out count);
                var orderResult = GetOrder(fileterResult, pageInfo.SortField, pageInfo.Sort);
                var result = orderResult
                    .Skip((pageInfo.PageNumber - 1) * pageInfo.PageSize)
                    .Take(pageInfo.PageSize);

                return new
                {
                    PageCount = pageInfo.PageSize,
                    Data = result,
                    CurrentPage = pageInfo.PageNumber,
                    TotalCount = count
                };
            });
        }

        [HttpPost]
        public JsonResult Update(Address address)
        {
            if (!ModelState.IsValid)
                return JsonValidationError();

            return GetJsonResult(() =>
            {
                Address addr;
                if (address.Id == 0)
                {
                    addr = _context.Address.Add(address);
                }
                else
                {
                    addr = _context.Address.FirstOrDefault(p => p.Id == address.Id);
                    if (addr == null)
                        throw new NullReferenceException();
                    address.CopyTo(addr);
                }
                _context.SaveChanges();

                return addr;
            });
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            var addr = _context.Address.FirstOrDefault(p => p.Id == id);
            if (addr == null)
                return JsonError("刪除的項目 id: {0} 無法對應到地址項目".FormatWith(id));

            _context.Address.Remove(addr);

            return GetJsonResult(() =>
            {
                _context.SaveChanges();
            });
        }

        /// <summary>
        /// 依據欄位與排序順序，針對任意的 IQueryable or DbSet 項目進行排序
        /// </summary>
        /// <typeparam name="T">任意的型態</typeparam>
        /// <param name="set"></param>
        /// <param name="fieldName">排序的欄位，如果不指定，則使用 T 的第一個欄位；建議一定要指定</param>
        /// <param name="order">只能是 "ASC" or "DESC" 這兩種字串</param>
        /// <returns>傳回 IOrderedQueryable<T> 的型態</returns>
        private IQueryable<T> GetOrder<T>(IQueryable<T> set, string fieldName, string order)
        {
            fieldName = fieldName ?? typeof(T).GetProperties().First().Name;
            return set.OrderBy(fieldName + " " + order);
        }

        private IQueryable<T> GetStartWithFilter<T>(IQueryable<T> set, string searchField, string searchKeyword, out int count)
        {
            if (!string.IsNullOrEmpty(searchField) && !string.IsNullOrEmpty(searchKeyword))
            {
                set = set.Where(searchField + ".StartsWith(@0)", searchKeyword);
            }

            count = set.Count();
            return set;
        }
    }
}