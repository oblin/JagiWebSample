using Jagi.Interface;
using Jagi.Mvc.Helpers;
using JagiWebSample.Areas.Admin.Models;
using System.Web.Mvc;
using System.Linq;

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
                var result = _context.Address.OrderBy(o => o.Zip)
                    .Skip(pageInfo.CurrentPage * pageInfo.PageCount)
                    .Take(pageInfo.PageCount);

                return new 
                {
                    PageCount = pageInfo.PageCount,
                    Data = result,
                    CurrentPage = pageInfo.CurrentPage,
                    TotalCount = _context.Address.Count()
                };
            });
        }
    }
}