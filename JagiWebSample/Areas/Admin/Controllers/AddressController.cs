using Jagi.Interface;
using Jagi.Mvc.Helpers;
using JagiWebSample.Areas.Admin.Models;
using System.Web.Mvc;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading;
using System;

namespace JagiWebSample.Areas.Admin.Controllers
{
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
                var orderResult = GetOrder(_context.Address, pageInfo.SortField, pageInfo.Sort);
                var result = orderResult
                    .Skip((pageInfo.PageNumber - 1) * pageInfo.PageSize)
                    .Take(pageInfo.PageSize);

                return new 
                {
                    PageCount = pageInfo.PageSize,
                    Data = result,
                    CurrentPage = pageInfo.PageNumber,
                    TotalCount = _context.Address.Count()
                };
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
    }
}