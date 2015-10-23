using AutoMapper.QueryableExtensions;
using Jagi.Interface;
using JagiWebSample.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace JagiWebSample.Areas.Admin.Controllers
{
    public class ColumnsController : ControllerBase
    {
        private AdminDataContext _context;

        public ColumnsController(AdminDataContext context)
        {
            _context = context;
        }

        public ActionResult Index(string id)
        {
            var routeData = RouteData;

            var tableNames = _context.TableSchema.Select(s => s.TableName).Distinct().ToList();
            var result = new TableSchemaGroupView
            {
                TableName = id,
                TableNames = tableNames,
                Schema = new TableSchemaListView()
            };
            return View(result);
        }

        public void Create()
        {

        }

        [HttpGet]
        public JsonResult GetList(string id)
        {
            var result = _context.TableSchema.Where(k => k.TableName == id);
            return JsonSuccess(result);
        }

        private string FindDefaultDisplayName(TableSchema column)
        {
            string defaultTableName = ConstantString.SCHEMA_DEFAULT_TABLE_NAME;
            var defaultColumn = _context.TableSchema
                .SingleOrDefault(k => k.TableName == defaultTableName && k.ColumnName == column.ColumnName);
            if (defaultColumn != null)
                return defaultColumn.DisplayName;
            return string.Empty;
        }
    }
}