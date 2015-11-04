using AutoMapper;
using Jagi.Database.Cache;
using Jagi.Database.Mvc;
using Jagi.Interface;
using Jagi.Helpers;
using Jagi.Mvc;
using JagiWebSample.Areas.Admin.Models;
using System.Linq;
using System.Web.Mvc;

namespace JagiWebSample.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0)]
    public class ColumnsController : JagiController
    {
        private AdminDataContext _context;
        private ColumnsCache _columns;

        public ColumnsController(AdminDataContext context)
        {
            _context = context;
            _columns = new ColumnsCache();
        }

        public ActionResult Index(string id)
        {
            var tableNames = _context.TableSchema.Select(s => s.TableName).Distinct().ToList();
            var result = new TableSchemaGroupView
            {
                TableName = id,
                TableNames = tableNames,
                Schema = new TableSchemaListView()
            };
            return View(result);
        }

        public ActionResult Create()
        {
            var column = new TableSchemaEditView
            {
                TableName = ConstantString.SCHEMA_DEFAULT_TABLE_NAME
            };
            return View(column);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(TableSchemaEditView model)
        {
            if (!ModelState.IsValid)
                return View(model).WithError("新增失敗，請查閱錯誤原因");

            var isExist = _context.TableSchema.Any(k => k.ColumnName == model.ColumnName && k.TableName == model.TableName);
            if (isExist)
                return View(model).WithError("已經有相同的欄位定義，請使用修改方式");

            var tableSchema = Mapper.Map<TableSchema>(model);
            _context.TableSchema.Add(tableSchema);
            _context.SaveChanges();

            UpdateColumnCache(tableSchema);

            return RedirectToAction<ColumnsController>(c => c.Index(model.TableName)).WithSuccess("新增成功");
        }

        public ActionResult Edit(int id)
        {
            var column = _context.TableSchema.SingleOrDefault(k => k.Id == id);

            if (string.IsNullOrEmpty(column.DisplayName))
                column.DisplayName = FindDefaultDisplayName(column);

            var viewMode = Mapper.Map<TableSchemaEditView>(column);

            return View(viewMode);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(TableSchemaEditView model)
        {
            if (!ModelState.IsValid)
                return View(model).WithError("修改失敗，請查閱錯誤原因");

            var tableSchema = _context.TableSchema.SingleOrDefault(k => k.Id == model.Id);

            if (tableSchema == null)
                return View(model).WithError("找不到該筆資料，請重新操作");

            tableSchema = Mapper.Map<TableSchemaEditView, TableSchema>(model, tableSchema);
            _context.SaveChanges();

            UpdateColumnCache(tableSchema);

            return RedirectToAction<ColumnsController>(c => c.Index(tableSchema.TableName))
                    .WithSuccess("修改成功");
            //return RedirectToAction("Index", new { id = tableSchema.TableName })
            //    .WithSuccess("修改成功");
        }

        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var tableSchema = _context.TableSchema.SingleOrDefault(k => k.Id == id);
            if (tableSchema == null)
            {
                return RedirectToAction("Index").WithError("找不到該筆資料，無法刪除");
            }
            _columns.Remove(tableSchema.TableName, tableSchema.ColumnName);
            _context.TableSchema.Remove(tableSchema);
            _context.SaveChanges();

            return RedirectToAction<ColumnsController>(c => c.Index(tableSchema.TableName))
                .WithSuccess("刪除資料成功");
        }

        [HttpGet]
        public JsonResult GetList(string id)
        {
            var result = _context.TableSchema.Where(k => k.TableName == id);
            return JsonSuccess(result);
        }

        private void UpdateColumnCache(TableSchema tableSchema)
        {
            Jagi.Database.Models.TableSchema jgTableSchema = new Jagi.Database.Models.TableSchema();
            tableSchema.CopyTo(jgTableSchema);
            _columns.Set(jgTableSchema);
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