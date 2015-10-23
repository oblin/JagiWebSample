using Jagi.Helpers;
using Jagi.Interface;
using Jagi.Mvc;
using JagiWebSample.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace JagiWebSample.Areas.Admin.Controllers
{
    public class SchemaController : ControllerBase
    {
        private AdminDataContext _context;

        private string[] _excludeTables = new string[]
        {
            "__MigrationHistory", "AspNetRoles", "AspNetUserClaims",
            "AspNetUserLogins", "AspNetUserRoles", "AspNetUsers"
        };

        public SchemaController(AdminDataContext context)
        {
            _context = context;
        }

        public ActionResult Index()
        {
            var result = new List<SchemaTablesView>();
            var connectionString = _context.Database.Connection.ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                DataTable table = connection.GetSchema("Tables");
                foreach (DataRow row in table.Rows)
                {
                    if (_excludeTables.Contains(row["TABLE_NAME"].ToString()))
                        continue;

                    result.Add(new SchemaTablesView
                    {
                        CatalogName = row["TABLE_CATALOG"].ToString(),
                        SchemaName = row["TABLE_SCHEMA"].ToString(),
                        Name = row["TABLE_NAME"].ToString()
                    });
                }
            }
            return View(result);
        }

        public ActionResult Details(string id)
        {
            var result = new List<SchemaColumnsView>();
            GetSchemaColumns(id, result);
            return View(result);
        }

        [HttpPost]
        public ActionResult Details(FormCollection form)
        {
            var tableName = form["tableName"];
            var result = new List<SchemaColumnsView>();

            GetSchemaColumns(tableName, result);
            if (result.Count == 0)
                return View(result).WithWarning("查詢 {0} 資料表格無資料".FormatWith(tableName));
            var tableSchema = _context.TableSchema.Where(k => k.TableName == tableName).ToList();
            foreach (var column in result)
            {
                var defaultSchema = _context.TableSchema.SingleOrDefault(k =>
                    k.TableName == ConstantString.SCHEMA_DEFAULT_TABLE_NAME
                    && k.ColumnName == column.COLUMN_NAME);
                if (defaultSchema != null)          // 判斷已經有預設的欄位，則不在加入
                    continue;

                var columnSchema = tableSchema.SingleOrDefault(k =>
                    k.TableName == column.TABLE_NAME && k.ColumnName == column.COLUMN_NAME);
                if (columnSchema == null)
                    columnSchema = InsertIntoTableSchema(column);
                else
                    UpdateTableSchema(column, columnSchema);
            }

            _context.SaveChanges();
            return View(result).WithSuccess("建立 {0} 資料結構成功".FormatWith(tableName));
            //try
            //{
            //    _context.SaveChanges();
            //    return View(result).WithSuccess("建立 {0} 資料結構成功".FormatWith(tableName));
            //}
            //catch (Exception e)
            //{
            //    return RedirectToAction("Index").WithError(e.Message);
            //}
        }

        private TableSchema InsertIntoTableSchema(SchemaColumnsView column)
        {
            var entity = new TableSchema
            {
                TableName = column.TABLE_NAME,
                ColumnName = column.COLUMN_NAME,
                //DataType = (FieldType)Enum.Parse(typeof(FieldType), column.DATA_TYPE.ToUpper()),
                DataType = MappingFieldType(column.DATA_TYPE),
                Nullable = column.IS_NULLABLE == "YES" ? true : false,
                Precision = column.NUMERIC_PRECISION,
                StringMaxLength = column.CHARACTER_MAXIMUM_LENGTH,
                Scale = column.NUMERIC_SCALE
            };
            _context.TableSchema.Add(entity);
            return entity;
        }

        private void UpdateTableSchema(SchemaColumnsView column, TableSchema columnSchema)
        {
            //columnSchema.DataType = (FieldType)Enum.Parse(typeof(FieldType), column.DATA_TYPE.ToUpper());
            columnSchema.DataType = MappingFieldType(column.DATA_TYPE);
            columnSchema.Nullable = column.IS_NULLABLE == "YES" ? true : false;
            columnSchema.Precision = column.NUMERIC_PRECISION;
            columnSchema.StringMaxLength = column.CHARACTER_MAXIMUM_LENGTH;
            columnSchema.Scale = column.NUMERIC_SCALE;
        }

        private FieldType MappingFieldType(string dataType)
        {
            dataType = dataType.ToUpper();
            switch (dataType)
            {
                case "NVARCHAR":
                    return FieldType.String;

                case "DATETIME":
                    return FieldType.DateTime;

                case "INT":
                    return FieldType.Int32;

                case "BIT":
                    return FieldType.Boolean;

                case "DECIMAL":
                case "NUMERIC":
                    return FieldType.Decimal;

                case "TIMESTAMP":
                    return FieldType.ByteArray;

                default:
                    throw new InvalidCastException("無法轉換資料庫型態到對應的 FieldType，請重新定義");
            }
        }

        private void GetSchemaColumns(string id, List<SchemaColumnsView> result)
        {
            SqlDataReader reader = null;
            var connectionString = _context.Database.Connection.ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{0}'".FormatWith(id),
                    connection);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new SchemaColumnsView
                    {
                        TABLE_NAME = reader["TABLE_NAME"].ToString(),
                        COLUMN_NAME = reader["COLUMN_NAME"].ToString(),
                        DATA_TYPE = reader["DATA_TYPE"].ToString(),
                        IS_NULLABLE = reader["IS_NULLABLE"].ToString(),
                        CHARACTER_MAXIMUM_LENGTH = reader["CHARACTER_MAXIMUM_LENGTH"] is System.DBNull
                            ? 0
                            : Convert.ToInt16(reader["CHARACTER_MAXIMUM_LENGTH"]) == -1
                            ? Int32.MaxValue
                            : Convert.ToInt16(reader["CHARACTER_MAXIMUM_LENGTH"]),
                        COLUMN_DEFAULT = reader["COLUMN_DEFAULT"] is System.DBNull
                            ? string.Empty
                            : reader["COLUMN_DEFAULT"].ToString(),
                        NUMERIC_PRECISION = reader["NUMERIC_PRECISION"] is System.DBNull
                            ? 0
                            : Convert.ToInt16(reader["NUMERIC_PRECISION"]),
                        NUMERIC_SCALE = reader["NUMERIC_SCALE"] is System.DBNull
                            ? 0
                            : Convert.ToInt16(reader["NUMERIC_SCALE"])
                    });
                }
            }
        }
    }
}