using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;

namespace DZKSserver
{
    public class Excel
    {
        private string path = "";
        _Application excel = new _Excel.Application();
        Workbook wb;
        Worksheet ws;

        public Excel(string path, int Sheet)
        {
            this.path = path;
            wb = excel.Workbooks.Open(path);
            ws = wb.Worksheets[Sheet];
        }

        public string ReadCell(int i, int j)
        {
            i++;
            j++;
            if (ws.Cells[i,j].Value != null)
            {
                return ws.Cells[i,j].Value;
            }
            else
            {
                return "";
            }
        }
        public int GetRowCount()
        {
            int lastUsedRow = ws.Cells.Find("*", System.Reflection.Missing.Value,
                                           System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                                           _Excel.XlSearchOrder.xlByRows, _Excel.XlSearchDirection.xlPrevious,
                                           false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Row;
            
            return lastUsedRow;
        }
    }
}
