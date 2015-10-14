using AutoMapper;
using AutoMapper.QueryableExtensions;
using JagiWebSample.Areas.Admin.Models;
using System.Web.Mvc;

namespace JagiWebSample.Areas.Admin.Controllers
{
    public class CodesController : Controller
    {
        private AdminDataContext _context;

        public CodesController(AdminDataContext context)
        {
            _context = context;
        }

        public ActionResult Index()
        {
            var result = _context.CodeFiles.ProjectTo<CodeFilesEditView>();

            return View(result);
        }
    }
}