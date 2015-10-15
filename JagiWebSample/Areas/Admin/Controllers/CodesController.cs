using AutoMapper;
using AutoMapper.QueryableExtensions;
using Jagi.Mvc;
using JagiWebSample.Areas.Admin.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace JagiWebSample.Areas.Admin.Controllers
{
    public class CodesController : JagiControllerBase
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

        [HttpPost]
        public JsonResult Update(CodeFilesEditView model)
        {
            var target = _context.CodeFiles.Find(model.Id);
            Mapper.Map(model, target);

            _context.SaveChanges();

            return BetterJson(target);
        }

        [HttpPost]
        public JsonResult Add(CodeFilesEditView model)
        {
            var target = Mapper.Map<CodeFile>(model);
            _context.CodeFiles.Add(target);
            _context.SaveChanges();

            return BetterJson(Mapper.Map<CodeFilesEditView>(target));
        }

        [HttpGet]
        public JsonResult Details(int id)
        {
            var code = _context.CodeFiles.Find(id);
            var result = Mapper.Map<IEnumerable<CodeDetailEditView>>(code.CodeDetails);
            if (result == null)
                return JsonSuccess(new List<CodeDetailEditView>());

            return JsonSuccess(result);
        }
    }
}