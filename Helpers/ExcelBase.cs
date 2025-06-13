using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace EventMonitoring.Helpers
{
    public class ExcelBase : IDisposable
    {
        public ExcelBase()
        {

        }

        /// <summary>
        /// Accounting and recording data using rows and columns into which information can be entered.
        /// </summary>
        private static SpreadsheetDocument _spreadSheet = null;

        /// <summary>
        /// Single spreadsheet that contains cells organized by rows and columns.
        /// </summary>
        private static Worksheet _workSheet = null;

        /// <summary>
        /// Create Workbook via DataTable.
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="dt"></param>
        public void CreateWorkbook(MemoryStream ms, DataTable dt)
        {
            using (var workbook = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = workbook.AddWorkbookPart();

                workbook.WorkbookPart.Workbook = new Workbook
                {
                    Sheets = new Sheets()
                };

                var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                sheetPart.Worksheet = new Worksheet(sheetData);

                var sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                uint sheetId = 1;
                if (sheets.Elements<Sheet>().Count() > 0)
                {
                    sheetId =
                        sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                }

                var sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = "Sheet 1" };
                sheets.Append(sheet);

                var headerRow = new Row();

                var columns = new List<string>();
                foreach (DataColumn column in dt.Columns)
                {
                    columns.Add(column.ColumnName);

                    var cell = new Cell
                    {
                        DataType = CellValues.String,
                        CellValue = new CellValue(column.ColumnName)
                    };
                    headerRow.AppendChild(cell);
                }

                sheetData.AppendChild(headerRow);

                foreach (DataRow dsrow in dt.Rows)
                {
                    var newRow = new Row();
                    foreach (String col in columns)
                    {
                        var a = dsrow[col];

                        var cell = new Cell
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(dsrow[col].ToString())
                        };

                        newRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(newRow);
                }

                workbook.Close();
            }
        }

        /// <summary>
        /// Creates a new workbook.
        /// </summary>
        /// <param name="ms"></param>
        public void CreateWorkbook(MemoryStream ms)
        {
            _spreadSheet = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook, false);

            _spreadSheet.AddWorkbookPart();
            _spreadSheet.WorkbookPart.Workbook = new Workbook();
            _spreadSheet.WorkbookPart.Workbook.Save();

            _spreadSheet.WorkbookPart.Workbook.Sheets = new Sheets();
            _spreadSheet.WorkbookPart.Workbook.Save();
        }

        /// <summary>
        /// Adds a new worksheet to the workbook.
        /// </summary>
        /// <param name="workSheetName"></param>
        public void AddWorksheet(string workSheetName)
        {
            var sheets = _spreadSheet.WorkbookPart.Workbook.GetFirstChild<Sheets>();
            var worksheetPart = _spreadSheet.WorkbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());
            worksheetPart.Worksheet.Save();

            var sheet = new Sheet()
            {
                Id = _spreadSheet.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = (uint)(_spreadSheet.WorkbookPart.Workbook.Sheets.Count() + 1),
                Name = workSheetName
            };

            sheets.Append(sheet);
            _spreadSheet.WorkbookPart.Workbook.Save();
        }

        /// <summary>
        /// Sets a cell value. The row and the cell are created if they do not exist or If the cell exists, the contents of the cell is overwritten.
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <param name="rowIndex"></param>
        /// <param name="value"></param>
        public void SetCellValue(uint columnIndex, uint rowIndex, string value)
        {
            Column previousColumn = null;
            Cell previousCell = null;
            Row previousRow = null;
            Columns columns;
            Cell cell;
            Row row;

            _workSheet = _spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet;
            var sheetData = _workSheet.GetFirstChild<SheetData>();
            var cellAddress = ColumnNameFromIndex(columnIndex) + rowIndex;

            if (sheetData.Elements<Row>().Where(item => item.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(item => item.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };

                for (uint counter = rowIndex - 1; counter > 0; counter--)
                {
                    previousRow = sheetData.Elements<Row>().Where(item => item.RowIndex == counter).FirstOrDefault();
                    if (previousRow != null)
                    {
                        break;
                    }
                }

                sheetData.InsertAfter(row, previousRow);
            }

            if (row.Elements<Cell>().Where(item => item.CellReference.Value == cellAddress).Count() > 0)
            {
                cell = row.Elements<Cell>().Where(item => item.CellReference.Value == cellAddress).First();
            }
            else
            {
                for (uint counter = columnIndex - 1; counter > 0; counter--)
                {
                    previousCell = row.Elements<Cell>().Where(item => item.CellReference.Value == ColumnNameFromIndex(counter) + rowIndex).FirstOrDefault();
                    if (previousCell != null)
                    {
                        break;
                    }
                }

                cell = new Cell() { CellReference = cellAddress };
                row.InsertAfter(cell, previousCell);
            }

            columns = _workSheet.Elements<Columns>().FirstOrDefault();

            if (columns == null)
            {
                columns = _workSheet.InsertAt(new Columns(), 0);
            }
            if (columns.Elements<Column>().Where(item => item.Min == columnIndex).Count() == 0)
            {
                for (uint counter = columnIndex - 1; counter > 0; counter--)
                {
                    previousColumn = columns.Elements<Column>().Where(item => item.Min == counter).FirstOrDefault();
                    if (previousColumn != null)
                    {
                        break;
                    }
                }
                columns.InsertAfter(new Column()
                {
                    Min = columnIndex,
                    Max = columnIndex,
                    CustomWidth = true,
                    Width = 20
                }, previousColumn);
            }

            cell.CellValue = new CellValue(value);

            cell.DataType = new EnumValue<CellValues>(CellValues.String);

            _workSheet.Save();
        }

        /// <summary>
        /// Converts a column number to column name (i.e. A, B, C..., AA, AB...).
        /// </summary>
        /// <param name="columnIndex"></param>
        private string ColumnNameFromIndex(uint columnIndex)
        {
            var columnName = string.Empty;

            while (columnIndex > 0)
            {
                var remainder = (columnIndex - 1) % 26;
                columnName = Convert.ToChar(65 + remainder).ToString() + columnName;
                columnIndex = (columnIndex - remainder) / 26;
            }

            return columnName;
        }

        /// <summary>
        /// Invoked to execute code required for memory cleanup and release and reset unmanaged resources, such as file handles and database connections. 
        /// </summary>
        public void Dispose()
        {
            _workSheet.Save();
            _spreadSheet.Close();
        }
    }
}