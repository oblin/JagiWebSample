﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Jagi.Mvc;
using JagiWebSample.Areas.Admin.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace JagiWebSample.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0)]
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
        public JsonResult Save(CodeFilesEditView model)
        {
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

        [HttpPost]
        public JsonResult SaveDetail(CodeDetailEditView model)
        {
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

            _context.SaveChanges();

            return JsonSuccess(Mapper.Map<CodeDetailEditView>(detail));
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