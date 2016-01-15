using LinqToExcel;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jagi.Helpers
{
    public class ExcelHelper
    {
        public static MemoryStream WriteToExcelStream<T>(List<T> items,
            string[] excludeFields, Dictionary<string, string> fieldsMappings, bool isHeading = true)
        {
            Type dataType = items.GetType().GetGenericArguments()[0];

            if (items == null || items.Count == 0 || dataType == null)
            {
                throw new Exception("處理範圍無資料，請重新輸入！");
            }

            var dataTypeProperties = dataType.GetProperties();

            //Create new Excel workbook
            var workbook = new HSSFWorkbook();

            //Create new Excel sheet
            var sheet = workbook.CreateSheet();

            // Create a header row
            var headerRow = sheet.CreateRow(0);
            int columnNumber = 0;
            foreach (var property in dataTypeProperties)
            {
                string cellName = property.Name;

                if (excludeFields != null && excludeFields.Contains(cellName))
                    continue;

                object[] attributes = property.GetCustomAttributes(typeof(DisplayAttribute), true);
                if (attributes != null && attributes.Count() > 0)
                {
                    DisplayAttribute attribute = attributes[0] as DisplayAttribute;
                    cellName = attribute.Name ?? property.Name;
                }
                string headName = cellName;

                if (isHeading)
                    if (fieldsMappings.Keys.Contains(cellName))
                        headName = fieldsMappings[cellName];

                headerRow.CreateCell(columnNumber++).SetCellValue(headName);
            }

            int rowNumber = 1;
            sheet.CreateFreezePane(0, 1, 0, 1);

            //Populate the sheet with values from the grid data
            foreach (object item in items)
            {
                //Create a new row
                var row = sheet.CreateRow(rowNumber++);

                int cellNumber = 0;
                foreach (var property in dataTypeProperties)
                {
                    if (excludeFields != null && excludeFields.Contains(property.Name))
                        continue;

                    object propertyValue = item.GetType().GetProperty(property.Name).GetValue(item, null);
                    string propertyValueString = ConvertToString(property.Name, propertyValue);
                    if (string.IsNullOrEmpty(propertyValueString))
                    {
                        row.CreateCell(cellNumber++).SetCellType(NPOI.SS.UserModel.CellType.Blank);
                    }
                    else
                    {
                        row.CreateCell(cellNumber++).SetCellValue(propertyValueString);
                    }
                }
            }

            // Autosize all columns
            for (int i = 0; i < columnNumber; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            //Write the workbook to a memory stream
            MemoryStream output = new MemoryStream();
            workbook.Write(output);
            return output;
        }

        public static List<T> RetrieveFromExcel<T>(string filename, string sheet = null)
        {
            var excelFile = new ExcelQueryFactory(filename);
            if (string.IsNullOrEmpty(sheet))
                sheet = "Sheet0";
            var excel = excelFile.Worksheet<T>(sheet);
            return excel.ToList();
        }

        private static string ConvertToString(string name, object propertyValue)
        {
            if (propertyValue == null)
                return string.Empty;

            string result = propertyValue.ToString();

            return result;
        }
    }
}
