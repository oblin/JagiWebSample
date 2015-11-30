using Jagi.Database.Mvc;
using Jagi.Helpers;
using Jagi.Interface;
using JagiWebSample.Areas.Admin.Models;
using System;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;

namespace JagiWebSample.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0)]
    public class AddressController : JagiController
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
                pageInfo.SortField = pageInfo.SortField ?? "Zip";

                var fileterResult = LinqHelper.StartWithFieldName(_context.Address, pageInfo.SearchField, pageInfo.SearchKeyword);
                int count = fileterResult.Count();

                var orderResult = LinqHelper.OrderByFieldName(fileterResult, pageInfo.SortField, pageInfo.Sort);
                var thenByResult = (orderResult as IOrderedQueryable<Address>).ThenBy(p => p.Id);
                var result = TakePagedResult(thenByResult, pageInfo);

                return new PagedView
                {
                    PageSize = pageInfo.PageSize,
                    Data = result,
                    PageNumber = pageInfo.PageNumber,
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
    }
}