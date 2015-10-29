using AutoMapper;
using AutoMapper.QueryableExtensions;
using Jagi.Database.Mvc;
using Jagi.Mvc;
using JagiWebSample.Areas.Admin.Models;
using System.Collections.Generic;
using System.Threading;
using System.Web.Mvc;

namespace JagiWebSample.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0)]
    public class CodesController : JagiController
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
        public JsonResult Save(CodeFilesEditView model)
        {
            if (!ModelState.IsValid)
                return JsonValidationError();

            CodeFile target;
            if (model.Id == 0)
            {
                target = Mapper.Map<CodeFile>(model);
                _context.CodeFiles.Add(target);
            }
            else
            {
                target = _context.CodeFiles.Find(model.Id);
                Mapper.Map(model, target);
            }

            return GetJsonResult(() =>
            {
                _context.SaveChanges();

                return Mapper.Map<CodeFilesEditView>(target);
            });
        }

        [HttpPost]
        public JsonResult DeleteCode(int id)
        {
            var code = _context.CodeFiles.Find(id);
            if (code == null)
                return JsonError("刪除資料失敗，找不到資料");

            _context.CodeFiles.Remove(code);
            _context.SaveChanges();

            return JsonSuccess();
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

        [HttpPost]
        public JsonResult SaveDetail(CodeDetailEditView model)
        {
            if (!ModelState.IsValid)
                return JsonValidationError();

            CodeDetail detail;
            if (model.Id == 0)
            {
                detail = Mapper.Map<CodeDetail>(model);
                _context.CodeDetails.Add(detail);
            }
            else
            {
                detail = _context.CodeDetails.Find(model.Id);
                Mapper.Map(model, detail);
            }

            return GetJsonResult(() => {
                _context.SaveChanges();
                return Mapper.Map<CodeDetailEditView>(detail);
            });
        }

        [HttpPost]
        public JsonResult DeleteDetail(int id)
        {
            var detail = _context.CodeDetails.Find(id);
            _context.CodeDetails.Remove(detail);

            _context.SaveChanges();

            return JsonSuccess();
        }
    }
}