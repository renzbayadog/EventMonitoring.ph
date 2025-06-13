using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EventMonitoring.Helpers
{
    public class ExcelData
    {
        public string Filename { get; set; }
        public string File { get; set; }
    }

    /// <summary>
    /// Summary description for ExportExcelHelper
    /// </summary>
    public class ExportExcelHelper
    {
        private List<string[]> _querriedList = new List<string[]>();
        private List<string> _excelHeader;

        public ExportExcelHelper()
        {

        }

        public ExportExcelHelper(List<string[]> queriedList, List<string> excelHeader)
        {
            _querriedList = queriedList;
            _excelHeader = excelHeader;
        }

        /// <summary>
        /// Create Excel Work Book via DataTable.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public byte[] CreateExcelWorkBook(DataTable dt)
        {
            using (var ms = new MemoryStream())
            {
                var excel = new ExcelBase();

                excel.CreateWorkbook(ms, dt);

                return ms.ToArray();
            }
        }


        /// <summary>
        /// Create Excel Work Book via List of object.
        /// </summary>
        /// <returns></returns>
        public byte[] CreateExcelWorkBook()
        {
            using (var ms = new MemoryStream())
            {
                using (var excel = new ExcelBase())
                {
                    excel.CreateWorkbook(ms);
                    excel.AddWorksheet("Sheet1");
                    PopulateHeader(excel);
                    PopulateDataRow(excel);
                }

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Populate data for header name.
        /// </summary>
        /// <param name="excel"></param>
        
        public void PopulateHeader(ExcelBase excel)
        {
            var counter = 0;

            foreach (var item in _excelHeader)
            {
                counter++;

                excel.SetCellValue(Convert.ToUInt32(counter), Convert.ToUInt32(1), item);
            }
        }

        /// <summary>
        /// Populate data for sheets cells.
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="rowIndex"></param>
        /// <param name="index"></param>
        /// 

        public void PopulateDataRow(ExcelBase excel)
        {
            uint rowIndex = 2;
            foreach (var item in _querriedList)
            {
                uint cellIndex = 1;
                foreach (var data in item)
                {
                    excel.SetCellValue(cellIndex, rowIndex, data);
                    cellIndex++;
                }

                rowIndex++;
            }
        }
    }
}
